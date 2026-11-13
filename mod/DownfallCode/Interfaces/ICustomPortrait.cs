using Godot;

namespace Downfall.DownfallCode.Interfaces;

public interface ICustomPortrait
{
    // return null to keep the default portrait
    Texture2D? GetPortraitTexture();
}