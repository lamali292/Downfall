using BaseLib.Extensions;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
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

    protected override async Task AfterApplied(PlayerChoiceContext ctx, Creature? applier, CardModel? cardSource)
    {
        if (Owner.Player == null) return;
        await GuardianCmd.EnterDefensiveMode(ctx, Owner.Player);
        await PowerCmd.Apply<ThornsPower>(ctx, Owner, DynamicVars.Power<ThornsPower>().BaseValue, Owner, null);
    }

    public override bool ShouldClearBlock(Creature creature)
    {
        return creature != Owner;
    }

    protected override async Task AfterRemoved(PlayerChoiceContext ctx, Creature oldOwner)
    {
        if (oldOwner.Player == null) return;
        await GuardianCmd.LeaveDefensiveMode(ctx, oldOwner.Player);
        await PowerCmd.Apply<ThornsPower>(ctx, Owner, -DynamicVars.Power<ThornsPower>().BaseValue, Owner, null);
    }

    protected override async Task AfterEnergyReset(PlayerChoiceContext ctx, Player player)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Decrement(this);
    }
    
}