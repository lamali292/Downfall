param([switch]$Clean)

$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

if (-not (Test-Path "local.props")) {
    Write-Host "ERROR: local.props not found. Copy local.props.example to local.props." -ForegroundColor Red
    exit 1
}

if ($Clean) {
    Remove-Item -Recurse -Force .godot -ErrorAction SilentlyContinue
    Write-Host "Cleaned .godot"
}

Write-Host "=== Generating images (ImageGen) ==="
dotnet run --project ImageGen/ImageGen.csproj
if ($LASTEXITCODE -ne 0) { throw "ImageGen failed" }

Write-Host "=== Building Downfall ==="
dotnet build Downfall.csproj --nologo -v q
if ($LASTEXITCODE -ne 0) {
    Write-Host "Retrying Downfall (cold publicizer cache)..." -ForegroundColor Yellow
    dotnet build Downfall.csproj --nologo -v q
    if ($LASTEXITCODE -ne 0) { throw "Downfall build failed" }
}

$pubDir = ".godot\mono\temp\obj\Debug\PublicizedAssemblies"
$sts2Pub = Get-ChildItem -Path $pubDir -Recurse -Filter "sts2.dll" -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $sts2Pub) { throw "Publicized sts2.dll not found" }
Write-Host "Publicized assemblies ready"

$mods = @(
    "Automaton", "Awakened", "Champ", "Collector", "Gremlins",
    "Guardian", "Hexaghost", "Hermit", "SlimeBoss", "Snecko"
)

$failed = @()
foreach ($mod in $mods) {
    Write-Host "=== Building $mod ==="
    $ok = $false
    for ($retry = 0; $retry -lt 10; $retry++) {
        dotnet build "$mod.csproj" --nologo -v q
        if ($LASTEXITCODE -eq 0) {
            $ok = $true
            break
        }
        if ($retry -lt 2) {
            Write-Host "  Retry $($retry + 1)/10..." -ForegroundColor Yellow
        }
    }
    if ($ok) {
        Write-Host "  OK" -ForegroundColor Green
    } else {
        $failed += $mod
        Write-Host "  FAILED after 3 attempts" -ForegroundColor Red
    }
}

if ($failed.Count -eq 0) {
    Write-Host "`nALL $($mods.Count) MODS BUILT SUCCESSFULLY" -ForegroundColor Green
} else {
    Write-Host "`nFAILED: $($failed -join ', ')" -ForegroundColor Red
    exit 1
}
