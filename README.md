


# Downfall — Slay the Spire 2
## Build
### Requirements
- A Slay the Spire 2 installation
- A Godot Executable (Preferably to be [MegaDot v4.5.1](https://megadot.megacrit.com/); secondly to be [Godot v4.5.1](https://godotengine.org/download/archive/4.5.1-stable/))
- Extracted Slay the Spire 2 assets from [GDRE](https://github.com/GDRETools/gdsdecomp)  
### Setup
#### 1. Clone the repository
```bash
git clone https://github.com/lamali292/Downfall.git
```

#### 2. Set the local path arguments
Copy `local.props.example` to `local.props` and set the paths.

#### 3. Link the assets, run the ImageGen & compile setup script
```bash
link-assets.ps1
setup.ps1
```

#### 4. Pack the assets
```bash
dotnet publish PublishAll/PublishAll.csproj
```

### Re-compilation and re-packing
The compilation and packing process is split into different steps. Under different circumstances, you may only need to run some of the steps.

* Original images changed: Run the image generator.
* Codes changed: Compile the code.
* Assets changed: Pack the assets.

#### Run the image generator
The image generator can copy the images from `ImageGen/` folder to project folders.

```bash
dotnet run --project ImageGen/ImageGen.csproj
```

#### Compile
If the code is changed but the assets are not, you can just compile the code without packing the assets. This will significantly reduce the time needed for testing.

```bash
dotnet build Awakened.csproj
```

#### Pack
If the assets are changed, you need to pack the assets into `.pck` files. This process will take a long time.

**This process also automatically compiles the code**.

```bash
dotnet publish Awakened.csproj
```
