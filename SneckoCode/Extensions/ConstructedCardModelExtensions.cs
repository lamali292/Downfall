using BaseLib.Abstracts;
using BaseLib.Extensions;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.DynamicVars;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Extensions;

public static class ConstructedCardModelExtensions
{
    extension(ConstructedCardModel card)
    {
        public ConstructedCardModel WithMuddle(decimal val, decimal upgrade = 0)
        {
            card.WithVars(new MuddleVar(val).WithUpgrade(upgrade));
            card.WithKeyword(SneckoKeywords.Muddle);
            return card;
        }

        public ConstructedCardModel WithOverflow()
        {
            card.WithKeyword(SneckoKeywords.Overflow);
            return card;
        }

        public ConstructedCardModel WithGift(Gift gift)
        {
            if (card is not IHasGift giftCard) return card;
            if (giftCard.Gift != null) throw new InvalidOperationException("Gift already set");
            giftCard.Gift = gift;
            card.WithTip(SneckoTip.Gift);
            return card;
        }
    }
    
   
}