<div align="center">

# Downfall

**A mod for Slay the Spire 2.**

[![Steam Workshop](https://img.shields.io/badge/Steam_Workshop-Subscribe-171a21?style=for-the-badge&logo=steam&logoColor=white)](https://steamcommunity.com/sharedfiles/filedetails/?id=3747508091)

[![MegaDot](https://img.shields.io/badge/MegaDot-4.5.1-478CBF?style=for-the-badge&logo=godotengine&logoColor=white)](https://megadot.megacrit.com/)
[![Slay the Spire 2](https://img.shields.io/badge/Slay_the_Spire_2-Buy-E03C31?style=for-the-badge)](https://store.steampowered.com/app/2868840/Slay_the_Spire_2/)
[![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)

</div>

---

## Build

Requirements:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (the code uses C# 14)
- [PowerShell 7](https://learn.microsoft.com/powershell/scripting/install/installing-powershell) (`pwsh`) on Linux/macOS; Windows PowerShell works on Windows
- A Slay the Spire 2 installation
- [MegaDot v4.5.1](https://megadot.megacrit.com/) *(preferred)* or [Godot v4.5.1](https://godotengine.org/download/archive/4.5.1-stable/)

Run everything below in a terminal opened in the repository root.

**1. Clone**

```bash
git clone https://github.com/lamali292/Downfall.git
cd Downfall
```

**2. Create `local.props`** - copy `local.props.example` to `local.props` and set:

```xml
<Project>
    <PropertyGroup>
        <GodotPath>C:\...\MegaDot\MegaDot_v4.5.1_mono.exe</GodotPath>
        <Sts2Branch>beta</Sts2Branch> <!-- beta or public: which game install to use -->
        <Sts2BetaPath>C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2</Sts2BetaPath>
        <!-- <Sts2PublicPath>...</Sts2PublicPath> only needed to also use the public branch -->
    </PropertyGroup>
</Project>
```

The paths are the full `Slay the Spire 2` game folders. An old `local.props` with `SteamLibraryPath` no longer works.

**3. Set up and publish** (first time only)

```bash
build/setup.ps1         # images + compile everything
build/publish-all.ps1   # Godot import, pack, copy into the game's mods folder
```

On Linux/macOS prefix with `pwsh`. The first run takes several minutes. Expect many red `ERROR:` lines from the Godot import (for example about `game.tscn`) - they are harmless. It worked if the last line is `ALL PROJECTS PUBLISHED`.

## Rebuilding

| What changed | Command |
|:-------------|:--------|
| **Images** | `dotnet run --project ImageGen/ImageGen.csproj` |
| **Code only** | `build/build-all.ps1` or `dotnet build Snecko.csproj -c ExportDebug` |
| **Assets or localization** | `build/publish-all.ps1` or `dotnet publish Snecko.csproj -c ExportDebug` |

Use `-Projects Champ,Guardian` with either script to limit it to some projects. The projects are `Downfall`, `Automaton`, `Awakened`, `Champ`, `Guardian`, `Hermit`, `Hexaghost`, `SlimeBoss`, `Snecko` and `Collector`; each builds its own `.dll` / `.pck`.

## Editing scenes or UI in Godot

See the [scene editing setup guide](https://github.com/lamali292/Downfall/wiki/Setup:-Editing-Scenes) in the wiki.
