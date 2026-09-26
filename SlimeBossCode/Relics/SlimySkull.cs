using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Relics;

[Pool(typeof(SlimeBossRelicPool))]
public class SlimySkull : SlimeBossRelicModel
{
    public SlimySkull() : base(RelicRarity.Common)
    {
        WithVar("WeakIncrease", 1);
        WithTip<WeakPower>();
    }

    public override decimal ModifyPowerAmountGivenAdditive(PowerModel power, Creature giver, decimal amount,
        Creature? target,
        CardModel? cardSource)
    {
        return giver == Owner.Creature && power is WeakPower ? DynamicVars["WeakIncrease"].BaseValue : 0;
    }
}