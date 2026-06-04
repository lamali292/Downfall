# release.ps1  ->  bumps patch, sets version everywhere, builds, then releases
$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

$modIds    = @('Downfall','Snecko','Automaton','Champ','Collector','Awakened',
               'Gremlins','Hexaghost','Hermit','Guardian','SlimeBoss')
$propsFile = "mod.build.props"

# --- read current version, compute bump ---
$props = Get-Content $propsFile -Raw
if ($props -notmatch '<Version>(\d+)\.(\d+)\.(\d+)</Version>') {
    throw "Couldn't find <Version>X.Y.Z</Version> in $propsFile"
}
$current = "{0}.{1}.{2}" -f $matches[1],$matches[2],$matches[3]
$new     = "{0}.{1}.{2}" -f $matches[1],$matches[2],([int]$matches[3] + 1)
Write-Host "$current -> $new"

# --- fail early on a stale tag, before changing anything ---
if (git tag --list "v$new") { throw "Tag v$new already exists. Delete it (git tag -d v$new) or bump." }

# --- write mod.build.props ---
$props = $props -replace '<Version>\d+\.\d+\.\d+</Version>',                      "<Version>$new</Version>"
$props = $props -replace '<AssemblyVersion>\d+\.\d+\.\d+\.\d+</AssemblyVersion>', "<AssemblyVersion>$new.0</AssemblyVersion>"
$props = $props -replace '<FileVersion>\d+\.\d+\.\d+\.\d+</FileVersion>',         "<FileVersion>$new.0</FileVersion>"
Set-Content $propsFile $props -Encoding UTF8 -NoNewline

# --- write every manifest ---
foreach ($id in $modIds) {
    $path = "$id.json"
    if (-not (Test-Path $path)) { throw "Manifest not found: $path" }
    $json = Get-Content $path -Raw | ConvertFrom-Json
    $json.version = $new
    foreach ($dep in $json.dependencies) {
        if ($modIds -contains $dep.id) { $dep.min_version = $new }
    }
    $json | ConvertTo-Json -Depth 10 | Set-Content $path -Encoding UTF8
}
Write-Host "Set version in $($modIds.Count) manifests"

# --- build (Godot's exit-time errors are expected; the zip is the real success check) ---
dotnet publish PublishAll/PublishAll.csproj -c Release -p:Version=$new

$zip = "dist/Downfall-$new.zip"
if (-not (Test-Path $zip)) { throw "No zip at $zip -- build did not produce output. Nothing committed." }
if ((Get-Item $zip).LastWriteTime -lt (Get-Date).AddMinutes(-10)) {
    throw "$zip is stale (not rebuilt this run). Aborting."
}

# --- only now: commit, tag, upload ---
git add $propsFile
foreach ($id in $modIds) { git add "$id.json" }
git commit -m "Release v$new"
git tag "v$new"
git push origin HEAD --tags
gh release create "v$new" $zip --title "v$new" --generate-notes
Write-Host "Released v$new"