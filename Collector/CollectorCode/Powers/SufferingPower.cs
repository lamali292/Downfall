using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Powers;

public class SufferingPower : CollectorPowerModel
{
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext ctx, PowerModel power, decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (applier != Owner || power is not (VulnerablePower or WeakPower) || power.Owner == Owner) return;
        await PowerCmd.Apply<CollectorDoomPower>(ctx, power.Owner, Amount, Owner, null);
    }
}