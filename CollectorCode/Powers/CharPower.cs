using Collector.CollectorCode.Cards.Common;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Char = Collector.CollectorCode.Cards.Common.Char;

namespace Collector.CollectorCode.Powers;

public class CharPower : CollectorPowerModel, IModifyDamageAdditive
{
    public CharPower() : base(PowerType.Debuff)
    {
        WithCardTip<Char>();
    }
    
    public decimal ModifyDamageAdditiveCompability(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        return cardSource is Char && props.IsPoweredAttack() && target == Owner ? Amount : 0;
    }
}