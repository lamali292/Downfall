<#
.SYNOPSIS
    Builds Downfall, launches Slay the Spire 2 headlessly with the STS2Export mod armed to
    auto-export every one of Downfall's cards/relics/potions (real game text, not reconstructed),
    and writes the result under DownfallWeb/data/game-export/.

.NOTES
    Requires STS2Export built and present in the game's mods folder (see
    sts2-exporter/STS2Export.csproj - `dotnet build` there copies its DLL/pck in, but the pck
    export step races a Godot file-lock bug; if it errors, re-run it, or build the pck by hand:
        megadot --headless --import --quit --path sts2-exporter
        (wait a couple seconds)
        megadot --headless --export-pack "Windows Desktop" "<mods>/STS2Export/STS2Export.pck" --path sts2-exporter
    Also requires local.props (SteamLibraryPath) same as test.ps1, and Steam running.

.EXAMPLE
    ./build/export-web-data.ps1
    ./build/export-web-data.ps1 -NoBuild
#>
param(
    [switch]$NoBuild,
    [int]$TimeoutMinutes = 15
)
$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path $PSScriptRoot -Parent
Set-Location $ProjectRoot

if (-not (Test-Path "local.props")) { throw "local.props not found. Copy local.props.example to local.props." }
$props = [xml](Get-Content "local.props")
$steamLib = $props.Project.PropertyGroup.SteamLibraryPath

$exe = Join-Path $steamLib "common\Slay the Spire 2\SlayTheSpire2.exe"
if (-not (Test-Path $exe)) { throw "Game executable not found: $exe" }

$modsDir = Join-Path $steamLib "common\Slay the Spire 2\mods"
if (-not (Test-Path (Join-Path $modsDir "STS2Export\STS2Export.dll"))) {
    throw "STS2Export is not in $modsDir. Build it first: cd ..\sts2-exporter; dotnet build STS2Export.csproj"
}

if (-not $NoBuild) {
    Write-Host "=== Building Downfall ===" -ForegroundColor Cyan
    dotnet build Downfall.csproj --nologo -v q
    if ($LASTEXITCODE -ne 0) { throw "Build failed" }
}

$webRoot = Join-Path (Split-Path $ProjectRoot -Parent) "DownfallWeb"
$outDir = Join-Path $webRoot "data\game-export"
if (Test-Path $outDir) { Remove-Item $outDir -Recurse -Force }
New-Item -ItemType Directory -Path $outDir -Force | Out-Null

$env:STS2EXPORT_AUTO_RUN = "1"
$env:STS2EXPORT_OUTPUT   = $outDir
try {
    Write-Host "=== Launching game headlessly to export real card/relic/potion text ===" -ForegroundColor Cyan
    $proc = Start-Process -FilePath $exe -ArgumentList "--headless", "--audio-driver", "Dummy" -WorkingDirectory (Split-Path $exe) -PassThru
    if (-not $proc.WaitForExit($TimeoutMinutes * 60 * 1000)) {
        $proc.Kill()
        throw "Timed out after $TimeoutMinutes minutes; killed the game."
    }
}
finally {
    Remove-Item Env:STS2EXPORT_AUTO_RUN, Env:STS2EXPORT_OUTPUT -ErrorAction SilentlyContinue
}

$itemsFile = Join-Path $outDir "Downfall\items.json"
if (-not (Test-Path $itemsFile)) {
    $log = Join-Path $env:APPDATA "SlayTheSpire2\logs\godot.log"
    throw "No export was written (game exit code $($proc.ExitCode)). Check $log"
}

$items = Get-Content $itemsFile -Raw | ConvertFrom-Json
Write-Host ""
Write-Host ("Exported {0} cards, {1} relics, {2} potions -> {3}" -f `
    $items.cards.Count, $items.relics.Count, $items.potions.Count, $itemsFile) -ForegroundColor Green
