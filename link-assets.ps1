param([switch]$Force)

$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

# Parse AssetSourcePath from local.props
$localProps = "local.props"
if (-not (Test-Path $localProps)) {
    Write-Host "ERROR: local.props not found. Copy local.props.example to local.props first." -ForegroundColor Red
    exit 1
}

$xml = [xml](Get-Content $localProps)
$assetSourcePath = $xml.Project.PropertyGroup.AssetSourcePath
if (-not $assetSourcePath -or -not (Test-Path $assetSourcePath)) {
    Write-Host "ERROR: AssetSourcePath '$assetSourcePath' not found or does not exist. Check local.props." -ForegroundColor Red
    exit 1
}
Write-Host "Asset source: $assetSourcePath"

$folders = @("src", "images", "fonts", "localization", "materials", "models",
             "scenes", "animations", "banks", "debug_audio", "shaders", "themes", "addons")

foreach ($folder in $folders) {
    $linkPath = Join-Path $PSScriptRoot $folder
    $targetPath = Join-Path $assetSourcePath $folder

    if (-not (Test-Path $targetPath)) {
        Write-Host "SKIP $folder (target not found: $targetPath)" -ForegroundColor DarkGray
        continue
    }

    if (Test-Path $linkPath) {
        if ($Force) {
            Remove-Item -LiteralPath $linkPath -Recurse -Force
            Write-Host "  Removed existing: $folder" -ForegroundColor DarkGray
        } else {
            Write-Host "SKIP $folder (already exists)" -ForegroundColor DarkGray
            continue
        }
    }

    New-Item -ItemType Junction -Path $linkPath -Target $targetPath -Force | Out-Null
    Write-Host "LINKED $folder -> $targetPath" -ForegroundColor Green
}

Write-Host "`nSymlinks ready." -ForegroundColor Green
