# Shared helpers for the build/*.ps1 scripts. Dot-source it: . "$PSScriptRoot\common.ps1"
# Works on Windows PowerShell 5.1 and PowerShell 7 (pwsh) on Windows, Linux and macOS, so it avoids
# $IsWindows/$IsMacOS (undefined on 5.1), $env:TEMP/$env:APPDATA (Windows-only) and backslash paths.
. (Join-Path $PSScriptRoot "get-sts2-path.ps1")

# "Windows", "Linux" or "macOS".
function Get-HostOS {
    if ([Environment]::OSVersion.Platform -eq [PlatformID]::Win32NT) { return "Windows" }
    if ((uname) -eq "Darwin") { return "macOS" }
    return "Linux"
}

function Get-TempDir { return [IO.Path]::GetTempPath() }

# Loads local.props and checks the paths every script needs, with an actionable message instead of a
# failure deep inside Godot/MSBuild. Pass -NeedGame when the script uses the game install (tests, mods folder).
function Get-LocalProps([switch]$NeedGodot, [switch]$NeedGame, [string]$Branch = "") {
    if (-not (Test-Path "local.props")) { throw "local.props not found. Copy local.props.example to local.props and fill in the paths." }
    $props = [xml](Get-Content "local.props")
    $group = $props.Project.PropertyGroup
    if ($group.SteamLibraryPath -and -not ($group.Sts2BetaPath -or $group.Sts2PublicPath)) {
        throw ("local.props still uses SteamLibraryPath, which is no longer read. Replace it with Sts2Branch and " +
               "Sts2BetaPath/Sts2PublicPath (full 'Slay the Spire 2' game folder). See local.props.example.")
    }
    if ($NeedGodot -and (-not $group.GodotPath -or -not (Test-Path $group.GodotPath))) {
        throw "GodotPath in local.props is missing or doesn't exist: '$($group.GodotPath)'"
    }
    if ($NeedGame) {
        $gameDir = Get-Sts2Path $props $Branch
        if (-not (Test-Path $gameDir)) { throw "Slay the Spire 2 folder from local.props doesn't exist: '$gameDir'" }
    }
    return $props
}

# The game executable inside a game folder.
function Get-Sts2ExePath([string]$GameDir) {
    switch (Get-HostOS) {
        "Windows" { return Join-Path $GameDir "SlayTheSpire2.exe" }
        "macOS"   { return Join-Path $GameDir "SlayTheSpire2.app/Contents/MacOS/SlayTheSpire2" }
        default   { return Join-Path $GameDir "SlayTheSpire2" }
    }
}

# The mods folder inside a game folder (matches ModsPath in Directory.Build.targets).
function Get-Sts2ModsDir([string]$GameDir) {
    if ((Get-HostOS) -eq "macOS") { return Join-Path $GameDir "SlayTheSpire2.app/Contents/MacOS/mods" }
    return Join-Path $GameDir "mods"
}

# The game's Godot log, for "no result file was written" hints. Godot's user:// directory per platform;
# only the Windows location is verified.
function Get-Sts2LogPath {
    switch (Get-HostOS) {
        "Windows" { $base = $env:APPDATA }
        "macOS"   { $base = Join-Path $HOME "Library/Application Support" }
        default   { $base = if ($env:XDG_DATA_HOME) { $env:XDG_DATA_HOME } else { Join-Path $HOME ".local/share" } }
    }
    return Join-Path $base "SlayTheSpire2/logs/godot.log"
}

# Mirrors $Source onto $Destination (extra files in $Destination are removed), like robocopy /MIR.
function Sync-Directory([string]$Source, [string]$Destination) {
    if ((Get-HostOS) -eq "Windows") {
        robocopy $Source $Destination /MIR /NFL /NDL /NJH /NJS /NP | Out-Null
        if ($LASTEXITCODE -ge 8) { throw "robocopy failed ($LASTEXITCODE) syncing '$Source' to '$Destination'" }
        $global:LASTEXITCODE = 0
        return
    }
    if (Test-Path $Destination) { Remove-Item -Recurse -Force $Destination }
    New-Item -ItemType Directory -Force -Path (Split-Path $Destination -Parent) | Out-Null
    Copy-Item -Recurse -Force $Source $Destination
}
