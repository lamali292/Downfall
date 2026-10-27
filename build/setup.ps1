param([switch]$Clean)
$ErrorActionPreference = "Stop"

# This script lives in build/, but local.props, .godot, and Downfall.csproj
# live at the project root — one level up.
$ProjectRoot = Split-Path $PSScriptRoot -Parent
Set-Location $ProjectRoot

. (Join-Path $PSScriptRoot "common.ps1")
$null = Get-LocalProps -NeedGodot
if ($Clean) {
    Remove-Item -Recurse -Force .godot -ErrorAction SilentlyContinue
    Write-Host "Cleaned .godot"
}
Write-Host "=== Generating images (ImageGen) ==="
dotnet run --project ImageGen/ImageGen.csproj
if ($LASTEXITCODE -ne 0) { throw "ImageGen failed" }

Write-Host "=== Fetching spine-godot extension ===" -ForegroundColor Cyan
$spineUrl = "https://spine-godot.s3.eu-central-1.amazonaws.com/4.2/4.5.1-stable/spine-godot-extension-4.2-4.5.1-stable.zip"
$spineZip = Join-Path (Get-TempDir) "spine-godot-extension.zip"
$spineTmp = Join-Path (Get-TempDir) "spine-godot-extract"

# Skip if the extension is already installed
if (Test-Path (Join-Path "bin" "*.gdextension")) {
    Write-Host "spine-godot extension already present, skipping download"
} else {
    $ProgressPreference = "SilentlyContinue"   # makes Invoke-WebRequest much faster
    Invoke-WebRequest -Uri $spineUrl -OutFile $spineZip

    if (Test-Path $spineTmp) { Remove-Item -Recurse -Force $spineTmp }
    Expand-Archive -Path $spineZip -DestinationPath $spineTmp -Force

    # The zip has a top-level bin/ folder — merge its contents into ProjectRoot\bin
    New-Item -ItemType Directory -Force -Path "bin" | Out-Null
    Copy-Item -Path (Join-Path $spineTmp (Join-Path "bin" "*")) -Destination "bin" -Recurse -Force

    Remove-Item $spineZip -Force
    Remove-Item -Recurse -Force $spineTmp
    Write-Host "spine-godot extension installed to bin/" -ForegroundColor Green
}

Write-Host "=== Building every project (ExportDebug) ==="
& (Join-Path $PSScriptRoot "build-all.ps1") -Configuration ExportDebug

$pubDir = Join-Path ".godot" (Join-Path "mono" (Join-Path "temp" (Join-Path "obj" (Join-Path "Downfall" (Join-Path "ExportDebug" "PublicizedAssemblies")))))
$sts2Pub = Get-ChildItem -Path $pubDir -Recurse -Filter "sts2.dll" -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $sts2Pub) { throw "Publicized sts2.dll not found" }
Write-Host "Publicized assemblies ready"

Write-Host "`nDOWNFALL BUILT SUCCESSFULLY" -ForegroundColor Green
