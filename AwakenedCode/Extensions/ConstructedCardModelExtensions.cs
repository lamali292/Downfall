using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Powers;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Extensions;

public static class ConstructedCardModelExtensions
{
    public static ConstructedCardModel WithConjure(this ConstructedCardModel card, Func<CardModel, bool>? a = null)
    {
        if (a == null)
            card.WithTip(AwakenedTip.Conjure);
        else
            card.WithTips(e => a.Invoke(e) ? [HoverTipFactory.Static(AwakenedTip.Conjure)] : []);

        card.WithTags(AwakenedTag.Conjure);
        return card;
    }

    public static ConstructedCardModel WithDrained(this ConstructedCardModel card, int baseVal, int upgrade = 0)
    {
        card.WithPower<DrainedPower>(baseVal, upgrade, false);
        card.WithEnergy(baseVal, upgrade);
        return card;
    }
}