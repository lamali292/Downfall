using BaseLib.Utils;
using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Champ.ChampCode.Relics;

[Pool(typeof(ChampRelicPool))]
public class BerserkersGuide : ChampRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Common;
    // TODO
}