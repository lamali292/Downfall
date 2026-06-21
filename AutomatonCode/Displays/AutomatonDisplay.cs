using Automaton.AutomatonCode.Vfx;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Automaton.AutomatonCode.Displays;

public class AutomatonDisplay
{
    private static readonly Dictionary<Player, NSequenceDisplay> Displays = new();

    static AutomatonDisplay()
    {
        CombatManager.Instance.CombatEnded += _ =>
        {
            foreach (var d in Displays.Values)
                if (GodotObject.IsInstanceValid(d))
                    d.QueueFree();
            Displays.Clear();
        };
    }

    public static NSequenceDisplay? GetDisplay(Player player)
    {
        return Displays.GetValueOrDefault(player);
    }

    public static void Refresh(Player creature)
    {
        Displays.GetValueOrDefault(creature)?.Refresh();
    }


    private static void Register(Player creature, NSequenceDisplay display)
    {
        if (Displays.TryGetValue(creature, out var old))
            if (GodotObject.IsInstanceValid(old))
                old.QueueFree();

        Displays[creature] = display;
    }

    public static void SetupAutomatonUi(NCombatRoom combatRoom, Player player)
    {
        var display = NSequenceDisplay.Create(player);
        var vfxContainer = combatRoom.CombatVfxContainer;
        vfxContainer.AddChildSafely(display);

        var creatureNode = combatRoom.GetCreatureNode(player.Creature);
        if (creatureNode != null)
        {
            var globalTopPos = creatureNode.GetTopOfHitbox();
            display.Position = vfxContainer.GetGlobalTransform().AffineInverse() * globalTopPos;
            display.Position +=  LocalContext.IsMe(player) ? new Vector2(100f, -80f) :  new Vector2(20f, -80);
          
        }

        Register(player, display);
        display.Refresh();
    }
}