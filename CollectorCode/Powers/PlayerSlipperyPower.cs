using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class PlayerSlipperyPower : CollectorPowerModel
{
    
    public override decimal ModifyHpLostBeforeOsty(Creature target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || Amount <= 0 || amount <= 0m)
            return amount;

        return Math.Min(amount, 1m);
    }

    public override Task AfterModifyingHpLostBeforeOsty()
    {
        PowerCmd.Decrement(this);
        return Task.CompletedTask;
    }
}