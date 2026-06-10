# release.ps1  ->  bumps patch, sets version everywhere, builds, then releases
$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

$modIds    = @('Downfall','Snecko','Automaton','Champ','Collector','Awakened',
'Gremlins','Hexaghost','Hermit','Guardian','SlimeBoss')
$propsFile = "mod.build.props"
$probeProj = "Downfall.csproj"   # a normal mod csproj (NOT PublishAll) — it defines Sts2Path/SteamLibraryPath
$appId     = "2868840"

# --- resolve game + steam library the same way the build does (honours local.props + OS defaults) ---
$gameRoot  = (& dotnet msbuild $probeProj -getProperty:Sts2Path).Trim()
if ([string]::IsNullOrWhiteSpace($gameRoot)) { throw "Couldn't resolve Sts2Path from $probeProj (SDK 8.0.200+?)." }

$steamApps = (& dotnet msbuild $probeProj -getProperty:SteamLibraryPath).Trim()
if ([string]::IsNullOrWhiteSpace($steamApps)) { throw "Couldn't resolve SteamLibraryPath from $probeProj." }

# --- StS2 version from the game's own release_info.json (field includes leading 'v') ---
$relPath = Join-Path $gameRoot "release_info.json"
if (-not (Test-Path $relPath)) { throw "release_info.json not found at $relPath -- check the game path." }
$gameVersion     = (Get-Content $relPath -Raw | ConvertFrom-Json).version   # e.g. v0.107.0
if ([string]::IsNullOrWhiteSpace($gameVersion)) { throw "No 'version' in $relPath" }
$gameVersionBare = $gameVersion.TrimStart('v')                              # e.g. 0.107.0 (for manifests)

# --- read current mod version, compute bump ---
$props = Get-Content $propsFile -Raw
if ($props -notmatch '<Version>(\d+)\.(\d+)\.(\d+)</Version>') {
    throw "Couldn't find <Version>X.Y.Z</Version> in $propsFile"
}
$current = "{0}.{1}.{2}" -f $matches[1],$matches[2],$matches[3]
$new     = "{0}.{1}.{2}" -f $matches[1],$matches[2],([int]$matches[3] + 1)

# --- BaseLib version from <BaseLibVersion> in the props ---
if ($props -notmatch '<BaseLibVersion>([^<]+)</BaseLibVersion>') {
    throw "Couldn't find <BaseLibVersion> in $propsFile"
}
$baseLib = $matches[1].Trim()

Write-Host "$current -> $new  (StS2 $gameVersion, BaseLib $baseLib)"

# --- fail early on a stale tag, before changing anything ---
if (git tag --list "v$new") { throw "Tag v$new already exists. Delete it (git tag -d v$new) or bump." }

# --- write mod.build.props ---
$props = $props -replace '<Version>\d+\.\d+\.\d+</Version>',                      "<Version>$new</Version>"
$props = $props -replace '<AssemblyVersion>\d+\.\d+\.\d+\.\d+</AssemblyVersion>', "<AssemblyVersion>$new.0</AssemblyVersion>"
$props = $props -replace '<FileVersion>\d+\.\d+\.\d+\.\d+</FileVersion>',         "<FileVersion>$new.0</FileVersion>"
Set-Content $propsFile $props -Encoding UTF8 -NoNewline

# --- write every manifest (regex edits = original formatting preserved, clean diffs) ---
foreach ($id in $modIds) {
    $path = "$id.json"
    if (-not (Test-Path $path)) { throw "Manifest not found: $path" }
    $text = Get-Content $path -Raw

    # bump the top-level "version"
    $text = $text -replace '("version"\s*:\s*")\d+\.\d+\.\d+(")', "`${1}$new`$2"

    # set min_game_version to the game build we target
    $text = $text -replace '("min_game_version"\s*:\s*")[^"]+(")', "`${1}$gameVersionBare`$2"

    # bump min_version for deps that are our own mods
    foreach ($depId in $modIds) {
        $pattern = '("id"\s*:\s*"' + [regex]::Escape($depId) + '"\s*,\s*"min_version"\s*:\s*")\d+\.\d+\.\d+(")'
        $text = $text -replace $pattern, "`${1}$new`$2"
    }

    # set the BaseLib dependency's min_version  (CAVEAT: NuGet version may differ from in-game
    # BaseLib mod version the loader checks — comment this line out if they don't match)
    $text = $text -replace '("id"\s*:\s*"BaseLib"\s*,\s*"min_version"\s*:\s*")[^"]+(")', "`${1}$baseLib`$2"

    Set-Content $path $text -Encoding UTF8 -NoNewline
}
Write-Host "Set version in $($modIds.Count) manifests"

# --- build (Godot's exit-time errors are expected; the zip is the real success check) ---
dotnet publish PublishAll/PublishAll.csproj -c Release -p:Version=$new

$zip = "dist/Downfall-$new.zip"
if (-not (Test-Path $zip)) { throw "No zip at $zip -- build did not produce output. Nothing committed." }
if ((Get-Item $zip).LastWriteTime -lt (Get-Date).AddMinutes(-10)) {
    throw "$zip is stale (not rebuilt this run). Aborting."
}

# --- Steam branch detection ---
$acfPath = Join-Path $steamApps "appmanifest_$appId.acf"
$branch  = "default"
if (Test-Path $acfPath) {
    $content = Get-Content $acfPath -Raw
    if ($content -match '(?s)"UserConfig"\s*\{.*?"BetaKey"\s+"([^"]+)"') { $branch = $matches[1] }
} else {
    Write-Warning "Could not find manifest at $acfPath. Defaulting to 'default' branch."
}

# --- build the release body: banner + GitHub auto-notes ---
$display = "Downfall v$new - StS2 - $gameVersion"
$banner  = @"
Works on the '$branch' branch of Slay the Spire 2 (game version $gameVersion).
Works with BaseLib $baseLib.
"@

# --- commit + tag first; generate-notes reads the pushed tag ---
git add $propsFile
foreach ($id in $modIds) { git add "$id.json" }
git commit -m "Release v$new (StS2 $gameVersion, BaseLib $baseLib)"
git tag "v$new"
git push origin HEAD --tags

# --- auto notes for this tag, then prepend the banner ---
$repo = gh repo view --json nameWithOwner -q ".nameWithOwner"
$auto = gh api "repos/$repo/releases/generate-notes" -f tag_name="v$new" -q ".body"
$body = "$banner`n`n---`n`n$auto"

# --- create the release once, with combined body + display title ---
gh release create "v$new" "$zip" --title "$display" --notes "$body"
Write-Host "Released v$new"