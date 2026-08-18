using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Powers;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Extensions;

public static class ConstructedCardModelExtensions
{

    extension(ConstructedCardModel card)
    {
        public ConstructedCardModel WithConjure(Func<CardModel, bool>? a = null)
        {
            if (a == null)
                card.WithTip(AwakenedTip.Conjure);
            else
                card.WithTips(e => a.Invoke(e) ? [HoverTipFactory.Static(AwakenedTip.Conjure)] : []);

            card.WithTags(AwakenedTag.Conjure);
            return card;
        }

        public ConstructedCardModel WithDrained(int baseVal, int upgrade = 0)
        {
            card.WithPower<DrainedPower>(baseVal, upgrade, false);
            card.WithEnergy(baseVal, upgrade);
            return card;
        }
    }
    
  
}