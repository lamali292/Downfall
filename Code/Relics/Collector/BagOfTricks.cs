using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Downfall.Code.Relics.Collector;

[Pool(typeof(CollectorRelicPool))]
public class BagOfTricks : CollectorRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Common;
    // TODO
}