<#
.SYNOPSIS
    Builds the mod, launches Slay the Spire 2 with the in-game test runner armed,
    waits for it to finish, and prints the results. Exit code 0 = all passed.

.EXAMPLE
    ./build/test.ps1                     # run every [CardTest]
    ./build/test.ps1 -Filter Hermit      # only tests whose "Type.Method" contains "Hermit"
    ./build/test.ps1 -NoBuild -Filter CheatDeadOn

.NOTES
    Requires Steam running (the game exe is launched directly; steam_appid.txt is
    shipped with the game).
#>
param(
    [string]$Filter = "",
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

if (-not $NoBuild) {
    Write-Host "=== Building Downfall ===" -ForegroundColor Cyan
    dotnet build Downfall.csproj --nologo -v q
    if ($LASTEXITCODE -ne 0) { throw "Build failed" }
    
    Write-Host "=== Building SlimeBoss ===" -ForegroundColor Cyan
    dotnet build SlimeBoss.csproj --nologo -v q
    if ($LASTEXITCODE -ne 0) { throw "SlimeBoss build failed" }
    
    Write-Host "=== Building Automaton ===" -ForegroundColor Cyan
    dotnet build Automaton.csproj --nologo -v q
    if ($LASTEXITCODE -ne 0) { throw "Automaton build failed" }

    Write-Host "=== Building Hermit ===" -ForegroundColor Cyan
    dotnet build Hermit.csproj --nologo -v q
    if ($LASTEXITCODE -ne 0) { throw "Hermit build failed" }
    
    Write-Host "=== Building Awakened ===" -ForegroundColor Cyan
    dotnet build Awakened.csproj --nologo -v q
    if ($LASTEXITCODE -ne 0) { throw "Awakened build failed" }

    Write-Host "=== Building Champ ===" -ForegroundColor Cyan
    dotnet build Champ.csproj --nologo -v q
    if ($LASTEXITCODE -ne 0) { throw "Champ build failed" }
    
    Write-Host "=== Building Guardian ===" -ForegroundColor Cyan
    dotnet build Guardian.csproj --nologo -v q
    if ($LASTEXITCODE -ne 0) { throw "Guardian build failed" }

    Write-Host "=== Building Snecko ===" -ForegroundColor Cyan
    dotnet build Snecko.csproj --nologo -v q
    if ($LASTEXITCODE -ne 0) { throw "Snecko build failed" }

    Write-Host "=== Building Hexaghost ===" -ForegroundColor Cyan
    dotnet build Hexaghost.csproj --nologo -v q
    if ($LASTEXITCODE -ne 0) { throw "Hexaghost build failed" }

    Write-Host "=== Building Collector ===" -ForegroundColor Cyan
    dotnet build Collector.csproj --nologo -v q
    if ($LASTEXITCODE -ne 0) { throw "Collector build failed" }
}

$outFile = Join-Path $env:TEMP "downfall_tests.json"
Remove-Item $outFile -ErrorAction SilentlyContinue

$env:DOWNFALL_RUN_TESTS   = "1"
$env:DOWNFALL_TEST_FILTER = $Filter
$env:DOWNFALL_TEST_OUTPUT = $outFile
try {
    Write-Host "=== Launching game headlessly (filter: '$Filter') ===" -ForegroundColor Cyan
    $proc = Start-Process -FilePath $exe -ArgumentList "--headless", "--audio-driver", "Dummy" -WorkingDirectory (Split-Path $exe) -PassThru
    if (-not $proc.WaitForExit($TimeoutMinutes * 60 * 1000)) {
        $proc.Kill()
        throw "Timed out after $TimeoutMinutes minutes; killed the game."
    }
}
finally {
    Remove-Item Env:DOWNFALL_RUN_TESTS, Env:DOWNFALL_TEST_FILTER, Env:DOWNFALL_TEST_OUTPUT -ErrorAction SilentlyContinue
}

if (-not (Test-Path $outFile)) {
    $log = Join-Path $env:APPDATA "SlayTheSpire2\logs\godot.log"
    throw "No result file was written (game exit code $($proc.ExitCode)). Check $log"
}

$result = Get-Content $outFile -Raw | ConvertFrom-Json
Write-Host ""
Write-Host ("Seed {0}  |  {1:n1}s  |  {2} passed, {3} failed" -f `
    $result.Seed, ([TimeSpan]$result.Duration).TotalSeconds, $result.Passed.Count, $result.Failed.Count) `
    -ForegroundColor $(if ($result.Failed.Count -eq 0) { "Green" } else { "Red" })
foreach ($p in $result.Passed) { Write-Host "  PASS  $p" -ForegroundColor DarkGreen }
foreach ($f in $result.Failed) {
    Write-Host "  FAIL  $($f.Name)" -ForegroundColor Red
    Write-Host "        $($f.Message)"
}
Write-Host ""
Write-Host "Full JSON: $outFile"
exit $(if ($result.Failed.Count -eq 0) { 0 } else { 1 })
