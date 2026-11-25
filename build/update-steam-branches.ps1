# Checks and updates the two StS2 branch installs in D:\SteamBranches via steamcmd.
#   ./build/update-steam-branches.ps1 -Username <steam login>            # check, then update outdated branches
#   ./build/update-steam-branches.ps1 -Username <steam login> -CheckOnly # only report
# -Username can also come from the STEAM_USER env var (steamcmd reuses its cached login, so no password is needed
# after the first interactive login: run `steamcmd +login <user>` once).
param(
    [string]$Username = $env:STEAM_USER,
    [string]$Root = "D:\SteamBranches",
    [switch]$CheckOnly,
    [string]$SteamCmd = "C:\Users\lamal\AppData\Local\Microsoft\WinGet\Packages\Valve.SteamCMD_Microsoft.Winget.Source_8wekyb3d8bbwe\steamcmd.exe"
)

$AppId = 2868840
# folder -> steam branch name
$Branches = [ordered]@{
    "sts2-public" = "public"
    "sts2-beta"   = "public-beta"
}

if (-not $Username) { throw "Pass -Username <steam login> or set STEAM_USER." }
if (-not (Test-Path $SteamCmd)) { throw "steamcmd not found at $SteamCmd" }

function Get-InstalledBuild([string]$Folder) {
    $acf = Join-Path $Root "$Folder\Slay the Spire 2\steamapps\appmanifest_$AppId.acf"
    if (-not (Test-Path $acf)) { return $null }
    $m = Select-String -Path $acf -Pattern '^\s*"buildid"\s+"(\d+)"' | Select-Object -First 1
    if ($m) { return $m.Matches[0].Groups[1].Value }
}

# One steamcmd call fetches the latest build id of every branch.
$info = & $SteamCmd +login $Username +app_info_update 1 +app_info_print $AppId +quit | Out-String
$branchesBlock = [regex]::Match($info, '"branches"\s*\{(?<b>.*?\n\t\t\})', 'Singleline').Groups['b'].Value
if (-not $branchesBlock) { throw "Could not read branch info from steamcmd (login failed?). Output:`n$info" }

$outdated = @()
foreach ($folder in $Branches.Keys) {
    $branch = $Branches[$folder]
    $latest = [regex]::Match($branchesBlock, "`"$branch`"\s*\{\s*`"buildid`"\s*`"(\d+)`"").Groups[1].Value
    $current = Get-InstalledBuild $folder
    $state = if (-not $latest) { "UNKNOWN" } elseif ($current -eq $latest) { "up to date" } else { "OUTDATED" }
    "{0,-12} branch={1,-12} installed={2} latest={3}  {4}" -f $folder, $branch, $current, $latest, $state
    if ($state -eq "OUTDATED") { $outdated += $folder }
}

if ($CheckOnly -or $outdated.Count -eq 0) { return }

foreach ($folder in $outdated) {
    $branch = $Branches[$folder]
    Write-Host "Updating $folder ($branch)..."
    $dir = Join-Path $Root "$folder\Slay the Spire 2"
    # public branch is the default: omit -beta. Anything else needs it.
    $beta = if ($branch -eq "public") { @() } else { @("-beta", $branch) }
    & $SteamCmd +force_install_dir $dir +login $Username +app_update $AppId @beta validate +quit
    if ($LASTEXITCODE -ne 0) { Write-Error "steamcmd failed for $folder (exit $LASTEXITCODE)" }
}
