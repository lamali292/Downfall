using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Downfall.Code.Relics.Hexaghost;

[Pool(typeof(HexaghostRelicPool))]
public class SoulOfChaos : HexaghostRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Shop;
    // TODO
}