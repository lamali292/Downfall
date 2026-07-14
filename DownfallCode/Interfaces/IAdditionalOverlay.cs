using Godot;

namespace Downfall.DownfallCode.Interfaces;

public interface IAdditionalOverlay
{
    string OverlayNodeName { get; }
    Control? CreateAdditionalOverlay();
}
