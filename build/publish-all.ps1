<#
.SYNOPSIS
    Publishes Downfall + every internal submod + Collector in one go, importing Godot
    resources exactly once up front instead of once per project (each project's own
    GodotImport target would otherwise redundantly re-import the whole shared Godot project
    on every `dotnet publish`).

.EXAMPLE
    ./build/publish-all.ps1                        # publish everything, Release
    ./build/publish-all.ps1 -Configuration Debug
    ./build/publish-all.ps1 -Projects Champ,Guardian
    ./build/publish-all.ps1 -SkipImport            # reuse whatever's already in .godot/imported

.NOTES
    Mirrors what dual-branch-test.yml does in CI: import once, then publish every project
    with /p:SkipGodotImport=true so each one's own GodotImport target (Directory.Build.targets)
    no-ops instead of re-running the import.
#>
param(
    [string]$Configuration = "Release",
    [string[]]$Projects = @("Downfall", "Automaton", "Awakened", "Champ", "Guardian", "Hermit", "Hexaghost", "SlimeBoss", "Snecko", "Collector"),
    [switch]$SkipImport
)
$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path $PSScriptRoot -Parent
Set-Location $ProjectRoot

. (Join-Path $PSScriptRoot "common.ps1")
$props = Get-LocalProps -NeedGodot -NeedGame
$godotPath = $props.Project.PropertyGroup.GodotPath

if (-not $SkipImport) {
    $projectFiles = Get-ChildItem -Path $ProjectRoot -File | Where-Object { $_.Name -match '\.csproj$|^Directory\.Build\.(props|targets)$' }
    $snapshot = @{}
    foreach ($f in $projectFiles) { $snapshot[$f.FullName] = Get-Content $f.FullName -Raw }

    Write-Host "=== Importing Godot resources (once) ===" -ForegroundColor Cyan
    & $godotPath --headless --audio-driver Dummy --path $ProjectRoot --import --quit --extra-args --no-window

    & $godotPath --headless --audio-driver Dummy --path $ProjectRoot --import --quit --extra-args --no-window
    $importExitCode = $LASTEXITCODE

    $reverted = @()
    foreach ($path in $snapshot.Keys) {
        if ((Get-Content $path -Raw) -ne $snapshot[$path]) {
            Set-Content -Path $path -Value $snapshot[$path] -NoNewline
            $reverted += (Split-Path $path -Leaf)
        }
    }
    if ($reverted.Count -gt 0) {
        Write-Host "WARNING: Godot's editor rewrote these during import, reverted: $($reverted -join ', ')" -ForegroundColor Yellow
    }

    if ($importExitCode -ne 0) { throw "Godot import failed (exit $importExitCode)" }
}

foreach ($proj in $Projects) {
    Write-Host "=== Publishing $proj ($Configuration) ===" -ForegroundColor Cyan
    dotnet publish "$proj.csproj" -c $Configuration --nologo -v q /p:SkipGodotImport=true
    if ($LASTEXITCODE -ne 0) { throw "$proj publish failed" }
}

Write-Host "`nALL PROJECTS PUBLISHED" -ForegroundColor Green
