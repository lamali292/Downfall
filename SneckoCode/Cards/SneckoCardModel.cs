using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;

namespace Snecko.SneckoCode.Cards;

public abstract class SneckoCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool showInCardLibrary = true,
    bool autoAdd = true)
    : DownfallCardModel<Core.Snecko>(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
{
    protected override bool ShouldGlowGoldInternal =>
        Keywords.Contains(SneckoKeywords.Overflow) && SneckoCmd.OverflowActive(Owner, true);
}