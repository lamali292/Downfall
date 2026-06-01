using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Interfaces;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
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
}