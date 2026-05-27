using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Interfaces;
using Awakened.AwakenedCode.Powers;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using AwakenedCharacter = Awakened.AwakenedCode.Core.Awakened;

namespace Awakened.AwakenedCode.Cards;

public abstract class AwakenedCardModel : DownfallCardModel<AwakenedCharacter>
{
    protected AwakenedCardModel(
        int cost,
        CardType type,
        CardRarity rarity,
        TargetType targetType,
        bool showInCardLibrary = true,
        bool autoAdd = true)
        : base(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
    {
        WithTips(card => card is IChantable chantable
            ? chantable.HasChanted
                ? [HoverTipFactory.Static(AwakenedTip.Chanted)]
                : [HoverTipFactory.Static(AwakenedTip.Chant)]
            : []);
    }

    protected override bool ShouldGlowGoldInternal => this is IChantable chantable &&
                                                      (AwakenedCmd.WasLastCardPlayedPower(this) ||
                                                       chantable.HasChanted);

    protected AwakenedCardModel WithConjure(Func<CardModel, bool>? a = null)
    {
        if (a == null)
            WithTip(AwakenedTip.Conjure);
        else
            WithTips(e => a.Invoke(e) ? [HoverTipFactory.Static(AwakenedTip.Conjure)] : []);

        WithTags(AwakenedTag.Conjure);
        return this;
    }

    protected AwakenedCardModel WithDrained(int baseVal, int upgrade = 0)
    {
        this.WithPower<DrainedPower>(baseVal, upgrade, false);
        WithEnergy(baseVal, upgrade);
        return this;
    }
}