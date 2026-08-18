using BaseLib.Abstracts;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Hexaghost.HexaghostCode.Extensions;

public static class ConstructedCardModelExtensions
{
    extension(ConstructedCardModel card)
    {
        public ConstructedCardModel WithAfterlife()
        {
            card.WithKeywords(CardKeyword.Ethereal, HexaghostKeyword.Afterlife);
            return card;
        }
    }
    
    
}