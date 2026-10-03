using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;

namespace Hermit.HermitCode.Cards;

[Pool(typeof(HermitCardPool))]
public abstract class HermitCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool showInCardLibrary = true,
    bool autoAdd = true)
    : DownfallCardModel<Core.Hermit>(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
{
    protected ConstructedCardModel WithDeadOn()
    {
        WithKeyword(HermitKeywords.DeadOn);
        return this;
    }

    protected override bool ShouldGlowGoldInternal => HermitCmd.HasActiveDeadOnEffect(this);
}