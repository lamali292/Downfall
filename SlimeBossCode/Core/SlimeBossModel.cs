using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace SlimeBoss.SlimeBossCode.Core;

public class SlimeBossModel() : CustomSingletonModel(HookType.Combat)
{
    public override Task BeforeHandDraw(Player player, PlayerChoiceContext ctx,
        ICombatState combatState)
    {
        return SlimeBossCmd.CommandAll(ctx, player, ValueProp.Unpowered);
    }

    public override Task BeforeCombatStart()
    {
        SlimeQueue.ResetAllSlots();

        var state = CombatManager.Instance.DebugOnlyGetState();
        if (state == null) return Task.CompletedTask;
        foreach (var player in state.Players.Where(e => e.Character is SlimeBoss)) SlimeQueue.SetSlots(player, 3);
        return Task.CompletedTask;
    }
}