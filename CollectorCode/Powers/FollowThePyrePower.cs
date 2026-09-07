using Collector.CollectorCode.Cards.Common;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class FollowThePyrePower : CollectorPowerModel, IModifyDamageAdditive
{
    public FollowThePyrePower()
    {
        WithCardTip<FollowThePyre>();
    }
    
    public decimal ModifyDamageAdditiveCompability(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        return cardSource is FollowThePyre && props.IsPoweredAttack() && target == Owner ? Amount : 0;
    }
}