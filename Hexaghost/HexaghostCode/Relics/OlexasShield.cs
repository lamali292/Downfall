using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Hexaghost.HexaghostCode.Relics;

[Pool(typeof(HexaghostRelicPool))]
public class OlexasShield : HexaghostRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    // TODO
}