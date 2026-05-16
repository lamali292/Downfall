using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Relics;

[Pool(typeof(SlimeBossRelicPool))]
public class HeartOfGoo() : SlimeBossRelicModel(RelicRarity.Starter)
{
    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<BlackHeartOfGoo>();
    }
    // TODO
}