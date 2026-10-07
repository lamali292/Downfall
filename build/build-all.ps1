<#
.EXAMPLE
    ./build/build-all.ps1                       # every project, ExportDebug (default)
    ./build/build-all.ps1 -Configuration Debug   # Downfall only - pre-builds the editor-host assembly
    ./build/build-all.ps1 -Projects Champ,Guardian
    ./build/build-all.ps1 -Configuration Release
#>
param(
    [string]$Configuration = "ExportDebug",
    [string[]]$Projects = @("Downfall", "Automaton", "Awakened", "Champ", "Guardian", "Hermit", "Hexaghost", "SlimeBoss", "Snecko", "Collector")
)
$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path $PSScriptRoot -Parent
Set-Location $ProjectRoot

if ($Configuration -eq "Debug" -and ($Projects.Count -ne 1 -or $Projects[0] -ne "Downfall")) {
    Write-Host "NOTE: Debug only makes sense for Downfall itself (the Godot editor-host assembly) - restricting to it." -ForegroundColor Yellow
    $Projects = @("Downfall")
}

foreach ($proj in $Projects) {
    Write-Host "=== Building $proj ($Configuration) ===" -ForegroundColor Cyan
    dotnet build "$proj.csproj" -c $Configuration --nologo -v q
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Retrying $proj (cold publicizer cache)..." -ForegroundColor Yellow
        dotnet build "$proj.csproj" -c $Configuration --nologo -v q
        if ($LASTEXITCODE -ne 0) { throw "$proj build failed" }
    }
}

Write-Host "`nALL PROJECTS BUILT ($Configuration)" -ForegroundColor Green
