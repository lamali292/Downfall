using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

public class ProtectTheBossPower : SlimeBossPowerModel
{
    public override decimal ModifyHpLostAfterOstyLate(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        return target != Owner || Owner.Player == null || SlimeQueue.GetCount(Owner.Player) == 0 ? amount : 0M;
    }

    protected override async Task AfterModifyingHpLostAfterOsty(PlayerChoiceContext ctx)
    {
        if (Owner.Player == null) return;
        await PowerCmd.Decrement(this);
        await SlimeBossCmd.Absorb(ctx, Owner.Player);
    }
}