using BaseLib.Utils;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class TheContract() : CollectorRelicModel(RelicRarity.Shop)
{
    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained()
    {
        EssenceModel.AddEssence(Owner, 10);
        return Task.CompletedTask;
    }
}