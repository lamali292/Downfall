using Downfall.Code.Character;
using Downfall.Code.Displays;
using Downfall.Code.Vfx;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;

namespace Downfall.Code.Core;

public class AutomatonModel : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => true;

    
    public override Task AfterRoomEntered(AbstractRoom room)
    {
        var state = CombatManager.Instance.DebugOnlyGetState();
        if (state == null) return Task.CompletedTask;

        var combatRoomNode = NCombatRoom.Instance;
        if (combatRoomNode == null) return Task.CompletedTask;
        foreach (var player in state.Players)
        {
            if (player.Character is Automaton)
            {
                SetupAutomatonUi(combatRoomNode, player);
            }
        }
        return Task.CompletedTask;
    }

    private static void SetupAutomatonUi(NCombatRoom combatRoom, Player player)
    {
        var display = NSequenceDisplay.Create(player);
        var vfxContainer = combatRoom.CombatVfxContainer;
        vfxContainer.AddChildSafely(display);

        var creatureNode = combatRoom.GetCreatureNode(player.Creature);
        if (creatureNode != null)
        {
            var globalTopPos = creatureNode.GetTopOfHitbox();
            display.Position = vfxContainer.GetGlobalTransform().AffineInverse() * globalTopPos;
            display.Position += new Vector2(100f, -80f);
        }

        AutomatonDisplay.Register(player, display);
        display.Refresh();
    }
    
}