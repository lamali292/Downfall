using Automaton.AutomatonCode.Displays;
using Automaton.AutomatonCode.Events;
using Automaton.AutomatonCode.Extensions;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;

namespace Automaton.AutomatonCode.Core;

public class AutomatonRunModel() : CustomSingletonModel(HookType.Run)
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

public class AutomatonCombatModel() : CustomSingletonModel(HookType.Combat)
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        var modified = AutomatonHook.ModifyStashDraw(combatState, 1, player, out _);
        await StashCmd.DrawFromStash(player, modified);
    }
}