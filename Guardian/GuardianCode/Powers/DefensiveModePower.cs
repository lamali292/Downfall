using BaseLib.Extensions;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Powers;

public class DefensiveModePower : GuardianPowerModel
{
    public DefensiveModePower()
    {
        WithPower<ThornsPower>(3);
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (Owner.Player == null || LocalContext.NetId == null) return;
        var ctx = new HookPlayerChoiceContext(
            Owner.Player,
            LocalContext.NetId.Value,
            GameActionType.Combat);
        await GuardianCmd.EnterDefensiveMode(ctx, Owner.Player);
        await PowerCmd.Apply<ThornsPower>(ctx, Owner, DynamicVars.Power<ThornsPower>().BaseValue, Owner, null);
    }

    public override bool ShouldClearBlock(Creature creature)
    {
        return creature != Owner;
    }


    public override async Task AfterRemoved(Creature oldOwner)
    {
        if (oldOwner.Player == null || LocalContext.NetId == null) return;
        var ctx = new HookPlayerChoiceContext(
            oldOwner.Player,
            LocalContext.NetId.Value,
            GameActionType.Combat);
        await GuardianCmd.LeaveDefensiveMode(ctx, oldOwner.Player);
        await PowerCmd.Apply<ThornsPower>(ctx, Owner, -DynamicVars.Power<ThornsPower>().BaseValue, Owner, null);
    }

    public override async Task BeforeHandDrawLate(Player player, PlayerChoiceContext ctx,
        ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Decrement(this);
    }
}