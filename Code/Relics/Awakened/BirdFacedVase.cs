using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Downfall.Code.Relics.Awakened;

[Pool(typeof(AwakenedRelicPool))]
public class BirdFacedVase : AwakenedRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    // TODO
}