using BaseLib.Utils;
using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Champ.ChampCode.Relics;

[Pool(typeof(ChampRelicPool))]
public class DolphinsStyleGuide : ChampRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Shop;
    // TODO
}