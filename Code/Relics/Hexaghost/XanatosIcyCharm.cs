using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Downfall.Code.Relics.Hexaghost;

[Pool(typeof(HexaghostRelicPool))]
public class XanatosIcyCharm : HexaghostRelicModel
{
    // TODO - Boss relic
    public override RelicRarity Rarity => RelicRarity.Ancient;
}