using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Downfall.Code.Relics.Champ;

[Pool(typeof(ChampRelicPool))]
public class DefensiveThesis : ChampRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
}