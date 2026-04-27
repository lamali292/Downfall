using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Relics;

[Pool(typeof(SlimeBossRelicPool))]
public class GoopDweller : SlimeBossRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Common;
    // TODO
}