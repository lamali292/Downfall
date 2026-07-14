using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Interfaces;

public interface ICustomPowerIcon
{
    // Called on _Ready and again whenever RaiseIconChanged fires.
    // Draw onto the icon; lib clears previous decorations first.
    void DecorateIcon(TextureRect icon);

    // Lib subscribes to this for live updates. Implement as a plain event.
    event Action? IconChanged;
}



// Lib helper so implementers name their nodes consistently for cleanup:
public static class PowerIconExtensions
{
    public static void AddDecoration(this TextureRect icon, Control node, int index)
    {
        node.Name = $"_custom_icon_{index}";
        icon.AddChild(node);
    }
}