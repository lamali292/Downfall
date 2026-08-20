using Awakened.AwakenedCode.Vfx;
using Downfall.DownfallCode.Core;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Awakened.AwakenedCode.Displays;

public static class AwakenedDisplay
{
    private static readonly PlayerField<NSpellbookDisplay> Displays = new(() => null);

    public static bool HasDisplay(Player player)
    {
        return GodotObject.IsInstanceValid(Displays.Get(player));
    }

    public static void Register(Player player, NSpellbookDisplay display)
    {
        var old = Displays.Get(player);
        if (GodotObject.IsInstanceValid(old))
            old.QueueFree();

        Displays[player] = display;
    }

    public static void Refresh(Player player)
    {
        var display = Displays.Get(player);
        if (GodotObject.IsInstanceValid(display))
            display!.Refresh();
    }
}