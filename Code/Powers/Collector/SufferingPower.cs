using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Powers.Collector;

public class SufferingPower : CollectorPowerModel
{
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (applier != Owner || power is not (VulnerablePower or WeakPower) || power.Owner == Owner) return;
        await PowerCmd.Apply<CollectorDoomPower>(power.Owner, Amount, Owner, null);
        
    }
}