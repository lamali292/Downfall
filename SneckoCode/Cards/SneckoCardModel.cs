using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.DynamicVars;
using Snecko.SneckoCode.Interfaces;

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
        Keywords.Contains(SneckoKeywords.Overflow) && SneckoCmd.OverflowActive(this);


    public ConstructedCardModel WithMuddle(decimal val, decimal upgrade = 0)
    {
        WithVars(new MuddleVar(val).WithUpgrade(upgrade));
        WithKeyword(SneckoKeywords.Muddle);
        return this;
    }

    public ConstructedCardModel WithOverflow()
    {
        WithKeyword(SneckoKeywords.Overflow);
        return this;
    }

    public ConstructedCardModel WithGift(Gift gift)
    {
        if (this is not IHasGift giftCard) return this;
        if (giftCard.Gift != null) throw new InvalidOperationException("Gift already set");
        giftCard.Gift = gift;
        WithTip(SneckoTip.Gift);
        return this;
    }
}