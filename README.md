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

There are two ways to set up the project. Pick the one that matches what you want to do.

| Path | For you if… |
|:-----|:------------|
| **A · Code only** | You only want to write or change code, no editing scenes or UI in Godot. |
| **B · Full setup** | You want to open the project in Godot and edit scenes, UI, or other assets. |

> [!TIP]
> **Not sure?** Start with **Path A**. It's lighter, and you can switch to **Path B** later if you decide to touch the UI.

<br>
<details>
<summary><h3>Path A - Code only</h3></summary>

> For writing or changing code without editing scenes or UI in Godot.

#### Requirements

- A Slay the Spire 2 installation
- A Godot executable - [MegaDot v4.5.1](https://megadot.megacrit.com/) *(preferred)* or [Godot v4.5.1](https://godotengine.org/download/archive/4.5.1-stable/)

#### Setup

**1. Clone the repository**

```bash
git clone https://github.com/lamali292/Downfall.git
```

**2. Configure your local paths**

Copy `local.props.example` to `local.props` and fill in the values:

- **`GodotPath`** - the `.exe` of your downloaded MegaDot / Godot
- **`SteamLibraryPath`** - your `steamapps` folder, where Slay the Spire 2 is installed
- **`AssetSourcePath`** - *not needed for Path A*

```xml
<Project>
    <PropertyGroup>
        <GodotPath>C:\...\MegaDot\MegaDot_v4.5.1_mono.exe</GodotPath>
        <SteamLibraryPath>C:\Program Files (x86)\Steam\steamapps</SteamLibraryPath>
        <!-- AssetSourcePath is not required for Path A -->
    </PropertyGroup>
</Project>
```

**3. Run the setup script (ImageGen + build)**

```bash
build/setup.ps1
```

This builds the image atlas files and compiles the project. Add `-Clean` to wipe `.godot` first if you hit a stale build.

**4. Package and copy the project to the mods folder**

```bash
dotnet publish Downfall.csproj
```
</details>
<details>
<summary><h3>Path B - Full setup (scenes / UI)</h3></summary>

> For opening the project in Godot and editing scenes, UI, or other assets.

#### Requirements

- A Slay the Spire 2 installation
- A Godot executable - [MegaDot v4.5.1](https://megadot.megacrit.com/) *(preferred)* or [Godot v4.5.1](https://godotengine.org/download/archive/4.5.1-stable/)
- Extracted Slay the Spire 2 assets via [GDRE](https://github.com/GDRETools/gdsdecomp)

#### Setup

**1. Clone the repository**

```bash
git clone https://github.com/lamali292/Downfall.git
```

**2. Extract the game assets**

Extract Slay the Spire 2 using [GDRE](https://github.com/GDRETools/gdsdecomp). Note the root folder of the extracted project - you'll need it in the next step.

**3. Configure your local paths**

Copy `local.props.example` to `local.props` and fill in the values:

- **`GodotPath`** - the `.exe` of your downloaded MegaDot / Godot
- **`SteamLibraryPath`** - your `steamapps` folder, where Slay the Spire 2 is installed
- **`AssetSourcePath`** - the root folder of your extracted GDRE project

```xml
<Project>
    <PropertyGroup>
        <GodotPath>C:\...\MegaDot\MegaDot_v4.5.1_mono.exe</GodotPath>
        <SteamLibraryPath>C:\Program Files (x86)\Steam\steamapps</SteamLibraryPath>
        <AssetSourcePath>C:\path\to\extracted\Slay the Spire 2</AssetSourcePath>
    </PropertyGroup>
</Project>
```

**4. Link the assets, run ImageGen & the compile setup script**

```bash
build/link-assets.ps1
build/setup.ps1
```

**5. Pack the assets**

```bash
dotnet publish Downfall.csproj
```
</details>

<br>

## Re-compilation & re-packing

Once set up, both paths use the same commands. Depending on what you changed, you may only need to run one of them.

| What changed | What to run | Command |
|:-------------|:------------|:--------|
| **New images** | Image generator | `dotnet run --project ImageGen/ImageGen.csproj` |
| **Code only** | Compile | `dotnet build Downfall.csproj` |
| **Assets** (e.g. localization, images) | Pack | `dotnet publish Downfall.csproj` |

**New images** - build them into atlas files.

```bash
dotnet run --project ImageGen/ImageGen.csproj
```

**Code only** - code changed, assets unchanged. Much faster, ideal for quick testing.

```bash
dotnet build Downfall.csproj
```

**Assets** - repack after changing assets (e.g. localization). This takes a while and **automatically compiles the code too**.

```bash
dotnet publish Downfall.csproj
```
