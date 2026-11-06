using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using BaseLib.Abstracts;
using Downfall.DownfallCode.Abstract;
using Awakened.AwakenedCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using AwakenedCharacter = Awakened.AwakenedCode.Core.Awakened;

namespace Awakened.AwakenedCode.Cards;

public abstract class AwakenedCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool showInCardLibrary = true,
    bool autoAdd = true)
    : DownfallCardModel<AwakenedCharacter>(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
{
    protected override bool ShouldGlowGoldInternal =>
        Keywords.Contains(AwakenedKeyword.Chant) &&
        (ChantCmd.WasLastCardPlayedPower(this) || ChantCmd.HasChanted(this));

    protected ConstructedCardModel WithConjure(Func<CardModel, bool>? a = null)
    {
        if (a == null)
            WithTip(AwakenedTip.Conjure);
        else
            WithTips(e => a.Invoke(e) ? [HoverTipFactory.Static(AwakenedTip.Conjure)] : []);

        WithTags(AwakenedTag.Conjure);
        return this;
    }

    protected ConstructedCardModel WithDrained(int baseVal, int upgrade = 0)
    {
        WithPower<DrainedPower>(baseVal, upgrade, false);
        WithEnergy(baseVal, upgrade);
        return this;
    }
}