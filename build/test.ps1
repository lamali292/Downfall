<#
.SYNOPSIS
    Builds the mod, launches Slay the Spire 2 with the in-game test runner armed,
    waits for it to finish, and prints the results. Exit code 0 = all passed.

.EXAMPLE
    ./build/test.ps1                     # run every [CardTest]
    ./build/test.ps1 -Filter Hermit      # only tests whose "Type.Method" contains "Hermit"
    ./build/test.ps1 -NoBuild -Filter CheatDeadOn

.NOTES
    Requires "Test" in the Submods property of local.props, and Steam running
    (the game exe is launched directly; steam_appid.txt is shipped with the game).
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
$submods  = $props.Project.PropertyGroup.Submods
if ($submods -and ($submods -split ";") -notcontains "Test") {
    throw "Add 'Test' to <Submods> in local.props (currently: $submods)."
}

$exe = Join-Path $steamLib "common\Slay the Spire 2\SlayTheSpire2.exe"
if (-not (Test-Path $exe)) { throw "Game executable not found: $exe" }

if (-not $NoBuild) {
    Write-Host "=== Building Downfall ===" -ForegroundColor Cyan
    dotnet build Downfall.csproj --nologo -v q
    if ($LASTEXITCODE -ne 0) { throw "Build failed" }
}

$outFile = Join-Path $env:TEMP "downfall_tests.json"
Remove-Item $outFile -ErrorAction SilentlyContinue

$env:DOWNFALL_RUN_TESTS   = "1"
$env:DOWNFALL_TEST_FILTER = $Filter
$env:DOWNFALL_TEST_OUTPUT = $outFile
try {
    Write-Host "=== Launching game (filter: '$Filter') ===" -ForegroundColor Cyan
    $proc = Start-Process -FilePath $exe -WorkingDirectory (Split-Path $exe) -PassThru
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
