using Downfall.Code.Abstract;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Champ;

public class HoneBladePower : ChampPowerModel
{
    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        return !props.IsPoweredAttack() || cardSource == null || !cardSource.Tags.Contains(CardTag.Strike) ||
               (dealer != Owner && cardSource.Owner.Creature != Owner)
            ? 0M
            : Amount;
    }
}