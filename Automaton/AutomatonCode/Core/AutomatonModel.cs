using Automaton.AutomatonCode.Displays;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;

namespace Automaton.AutomatonCode.Core;

public class AutomatonModel() : CustomSingletonModel(true, true)
{
    public override Task AfterRoomEntered(AbstractRoom room)
    {
        var state = CombatManager.Instance.DebugOnlyGetState();
        var combatRoomNode = NCombatRoom.Instance;
        if (state == null || combatRoomNode == null) return Task.CompletedTask;
        foreach (var player in state.Players)
            if (player.Character is Automaton)
                AutomatonDisplay.SetupAutomatonUi(combatRoomNode, player);

        return Task.CompletedTask;
    }
}