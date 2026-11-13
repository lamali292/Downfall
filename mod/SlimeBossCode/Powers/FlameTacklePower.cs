using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Powers;

public class FlameTacklePower : SlimeBossPowerModel, IModifyDamageAdditive
{
    public decimal ModifyDamageAdditiveCompability(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        return dealer == Owner && cardSource != null && cardSource.Tags.Contains(SlimeBossTag.Tackle) &&
               target != dealer
            ? Amount
            : 0;
    }
}