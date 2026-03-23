# Downfall — Slay the Spire 2

# Dependency
This mod requires BaseLib-StS2 for better mod compatibility and future update support.

## Setup

### 1. Download the Repository

Clone the repository:

```bash
git clone https://github.com/lamali292/Downfall.git
```



### 2. Configure `Downfall.csproj`



Open the `Downfall.csproj` file and update the paths to match your system:

i.e in Windows look for

```xml
<PropertyGroup Condition="'$(IsWindows)' == 'true'">
  <GodotPath Condition="'$(GodotPath)' == ''">C:\Path\To\Godot\Godot4.5.1.exe</GodotPath>
  <SteamLibraryPath Condition="'$(SteamLibraryPath)' == ''">C:\Program Files (x86)\Steam\steamapps</SteamLibraryPath>
  <!-- The below should not need to be changed. -->
  ...
</PropertyGroup>
```



You only need to change:



- **SteamLibraryPath** → Path to your Steam Library

- **GodotPath** → Path to your [Godot 4.5.1 .Net exe](https://godotengine.org/download/archive/4.5.1-stable/)



### 3. Build the Mod



Build the project using your IDE or the .NET CLI.  

After building, mod is placed into your Slay the Spire 2 mods folder.

```xml
...\Steam\steamapps\common\Slay the Spire 2\mods\Downfall
```
