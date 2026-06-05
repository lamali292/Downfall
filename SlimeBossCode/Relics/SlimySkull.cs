using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Relics;

[Pool(typeof(SlimeBossRelicPool))]
public class SlimySkull : SlimeBossRelicModel
{
    public SlimySkull() : base(RelicRarity.Common)
    {
        WithVar("GoopIncrease", 1);
        WithTip<GoopPower>();
    }

    public override decimal ModifyPowerAmountGivenAdditive(PowerModel power, Creature giver, decimal amount, Creature? target,
        CardModel? cardSource)
    {
        return giver == Owner.Creature && power is GoopPower ? DynamicVars["GoopIncrease"].BaseValue : 0;
    }
}