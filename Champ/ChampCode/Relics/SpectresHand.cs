using BaseLib.Utils;
using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Champ.ChampCode.Relics;

[Pool(typeof(ChampRelicPool))]
public class SpectresHand : ChampRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    // TODO
}