param([switch]$Clean)
$ErrorActionPreference = "Stop"

# This script lives in build/, but local.props, .godot, and Downfall.csproj
# live at the project root — one level up.
$ProjectRoot = Split-Path $PSScriptRoot -Parent
Set-Location $ProjectRoot

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

Write-Host "=== Fetching spine-godot extension ===" -ForegroundColor Cyan
$spineUrl = "https://spine-godot.s3.eu-central-1.amazonaws.com/4.2/4.5.1-stable/spine-godot-extension-4.2-4.5.1-stable.zip"
$spineZip = Join-Path $env:TEMP "spine-godot-extension.zip"
$spineTmp = Join-Path $env:TEMP "spine-godot-extract"

# Skip if the extension is already installed
if (Test-Path "bin\*.gdextension") {
    Write-Host "spine-godot extension already present, skipping download"
} else {
    $ProgressPreference = "SilentlyContinue"   # makes Invoke-WebRequest much faster
    Invoke-WebRequest -Uri $spineUrl -OutFile $spineZip

    if (Test-Path $spineTmp) { Remove-Item -Recurse -Force $spineTmp }
    Expand-Archive -Path $spineZip -DestinationPath $spineTmp -Force

    # The zip has a top-level bin/ folder — merge its contents into ProjectRoot\bin
    New-Item -ItemType Directory -Force -Path "bin" | Out-Null
    Copy-Item -Path (Join-Path $spineTmp "bin\*") -Destination "bin" -Recurse -Force

    Remove-Item $spineZip -Force
    Remove-Item -Recurse -Force $spineTmp
    Write-Host "spine-godot extension installed to bin\" -ForegroundColor Green
}

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

Write-Host "`nDOWNFALL BUILT SUCCESSFULLY" -ForegroundColor Green