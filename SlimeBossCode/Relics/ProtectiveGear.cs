using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Relics;

[Pool(typeof(SlimeBossRelicPool))]
public class ProtectiveGear : SlimeBossRelicModel
{
    public ProtectiveGear() : base(RelicRarity.Shop)
    {
        WithVar("TackleReduce", 3);
    }


    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        return dealer == Owner.Creature && cardSource != null && cardSource.Tags.Contains(SlimeBossTag.Tackle) &&
               target == dealer
            ? -DynamicVars["TackleReduce"].BaseValue
            : 0;
    }
}