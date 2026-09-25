using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gremlins.GremlinsCode.Powers;

public class ScatterPower : GremlinsPowerModel
{
    public override decimal ModifyHpLostAfterOstyLate(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        return target != Owner ? amount : 0M;
    }

    public override async Task AfterModifyingHpLostAfterOsty()
    {
        if (Owner.Player == null) return;
        var ctx = new BlockingPlayerChoiceContext();
        await GremlinsCmd.SwapToRandom(ctx, Owner.Player);
        await PowerCmd.Decrement(this);
    }
}