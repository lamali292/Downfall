using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Champ.ChampCode.Powers;

public class HoneBladePower : ChampPowerModel
{
    public override decimal DownfallModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        return !props.IsPoweredAttack() || cardSource == null || !cardSource.Tags.Contains(CardTag.Strike) ||
               (dealer != Owner && cardSource.Owner.Creature != Owner)
            ? 0M
            : Amount;
    }
}