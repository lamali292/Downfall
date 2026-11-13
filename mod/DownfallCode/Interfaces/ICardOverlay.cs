using Godot;

namespace Downfall.DownfallCode.Interfaces;

public interface ICardOverlay
{
    // Called once when the overlay is first needed for this NCard.
    Control CreateCustomOverlay();

    // Called on every Reload and manual refresh. Sync visuals with model state.
    void UpdateOverlay(Control overlay);
}