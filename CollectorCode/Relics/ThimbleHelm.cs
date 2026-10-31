using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class ThimbleHelm : CollectorRelicModel, IModifyDamageAdditive
{
    public ThimbleHelm() : base(RelicRarity.Rare)
    {
        WithVar("TorchheadBonus", 4);
        WithTip(CollectorKeyword.Torchhead);
    }

    public decimal ModifyDamageAdditiveCompability(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer?.PetOwner != Owner || dealer.Monster is not TorchheadMonsterModel) return 0;
        return DynamicVars["TorchheadBonus"].IntValue;
    }
}