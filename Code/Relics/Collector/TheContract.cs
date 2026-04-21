using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Core.Collector;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Downfall.Code.Relics.Collector;

[Pool(typeof(CollectorRelicPool))]
public class TheContract : CollectorRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    public override Task AfterObtained()
    {
        EssenceModel.AddEssence(Owner, 10);
        return Task.CompletedTask;
    }
}