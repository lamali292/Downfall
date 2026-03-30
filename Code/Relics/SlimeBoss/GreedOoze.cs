using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Downfall.Code.Relics.SlimeBoss;

[Pool(typeof(SlimeBossRelicPool))]
public class GreedOoze : SlimeBossRelicModel
{
    // TODO - Event relic
    public override RelicRarity Rarity => RelicRarity.Event;
}