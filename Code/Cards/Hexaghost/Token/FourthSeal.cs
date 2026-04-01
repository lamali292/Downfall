using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Hexaghost.Token;

[Pool(typeof(TokenCardPool))]
public class FourthSeal : HexaghostCardModel
{
    public FourthSeal() : base(2, CardType.Power, CardRarity.Token, TargetType.None)
    {
        
    }
    // TODO: Implement
}