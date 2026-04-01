using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Relics.Awakened;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Relics.Hexaghost;

[Pool(typeof(HexaghostRelicPool))]
public class SpiritBrand : HexaghostRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    public override RelicModel GetUpgradeReplacement() => ModelDb.Relic<MarkOfTheEther>();
    // TODO
}