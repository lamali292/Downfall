using Downfall.Code.Abstract;
using Downfall.Code.Cards.Awakened.Token;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Awakened;

public class StormRulerPower : AwakenedPowerModel
{
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        if (dealer != Owner || cardSource is not Thunderbolt) return 0;
        return Amount;
    }
}