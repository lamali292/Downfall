using Downfall.Code.Cards.Vfx;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.Code.Displays;

public class AwakenedDisplay
{
    private static readonly Dictionary<Player, NSpellbookDisplay> Displays = new();

    public static void Register(Player player, NSpellbookDisplay display)
    {
        if (Displays.TryGetValue(player, out var old))
        {
            if (GodotObject.IsInstanceValid(old))
            {
                old.QueueFree();
            }
        }
        Displays[player] = display;
    }
    
    public static void Refresh(Player player)
    {
        Displays.GetValueOrDefault(player)?.Refresh();
    }
}