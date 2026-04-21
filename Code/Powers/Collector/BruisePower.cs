using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Collector;

public class BruisePower : DownfallPowerModel
{
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || amount < 0 || !props.IsPoweredAttack() || cardSource == null) return 0;
        return Amount;
    }
}