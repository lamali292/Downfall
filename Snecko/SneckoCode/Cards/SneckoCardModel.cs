using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.DynamicVars;
using Snecko.SneckoCode.Events;

namespace Snecko.SneckoCode.Cards;

public abstract class SneckoCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType)
    : DownfallCardModel<Core.Snecko>(cost, type, rarity, targetType)
{
    protected override bool ShouldGlowGoldInternal =>
        Keywords.Contains(SneckoKeywords.Overflow) && SneckoCmd.OverflowActive(Owner, true);

    public Gift? Gift { get; private set; }

    protected virtual async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await Task.CompletedTask;
    }


    protected virtual async Task OverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await Task.CompletedTask;
    }

    protected sealed override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PlayEffect(ctx, cardPlay);
        if (Keywords.Contains(SneckoKeywords.Overflow) && SneckoCmd.OverflowActive(Owner) && !cardPlay.IsAutoPlay)
        {
            await OverflowEffect(ctx, cardPlay);
            await SneckoHook.AfterOverflowEffect(CombatState!, cardPlay, this);
        }
    }


    protected ConstructedCardModel WithMuddle(decimal val, decimal upgrade = 0)
    {
        WithVars(new MuddleVar(val).WithUpgrade(upgrade));
        WithKeyword(SneckoKeywords.Muddle);
        return this;
    }

    protected ConstructedCardModel WithOverflow()
    {
        WithKeyword(SneckoKeywords.Overflow);
        return this;
    }

    protected ConstructedCardModel WithGift(Gift gift)
    {
        if (Gift != null) throw new InvalidOperationException("Gift already set");
        Gift = gift;
        return this;
    }
}