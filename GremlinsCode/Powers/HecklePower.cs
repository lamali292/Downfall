using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Gremlins.GremlinsCode.Powers;

public class HecklePower : GremlinsPowerModel
{
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext ctx, PowerModel power,
        decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (applier != Owner || power.Type != PowerType.Debuff || power.Owner == Owner || amount <= 0) return;
        await GremlinsCmd.GainTempHp(ctx, Owner, Amount);
    }
}