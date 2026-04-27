$source = "C:\Users\lamal\Desktop\Programmieren\sts2mods\Slay the Spire 2"
$target = "C:\Users\lamal\Desktop\Programmieren\sts2mods\Downfall\Automaton"

$folders = @(
    "shaders", "src", "themes", "addons",
    "animations", "banks", "debug_audio", "fonts", "images", "materials", "models", "scenes"
)

foreach ($folder in $folders) {
    $linkPath = Join-Path $target $folder
    $targetPath = Join-Path $source $folder
    New-Item -ItemType SymbolicLink -Path $linkPath -Target $targetPath
    Write-Host "Created symlink: $linkPath -> $targetPath"
}