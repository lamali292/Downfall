using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Hexaghost.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class GhostLash : HexaghostCardModel
{
    public GhostLash() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        
    }
    // TODO: Implement
}