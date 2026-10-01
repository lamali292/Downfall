using BaseLib.Utils;
using Downfall.DownfallCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Events;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Core;

/// <summary>
///     Snecko's play phases: whether Overflow is active is decided once, when the play starts, and
///     remembered on the play itself so the effect after the card resolves is not affected by changes made
///     during the play.
/// </summary>
public sealed class SneckoCardPlayPhases : ICardPlayPhases
{
    public static readonly SneckoCardPlayPhases Instance = new();

    private readonly SpireField<CardPlay, bool> _overflowAtPlayStart = new(() => false);

    public Task<bool> BeforePlay(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        _overflowAtPlayStart.Set(cardPlay, SneckoCmd.OverflowActive(card));
        return Task.FromResult(true);
    }

    public async Task AfterPlay(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (card is IHasOverflowEffect overflow
            && card.Keywords.Contains(SneckoKeywords.Overflow)
            && _overflowAtPlayStart[cardPlay])
        {
            await overflow.OverflowEffect(ctx, cardPlay);
            await SneckoHook.AfterOverflowEffect(card.Owner, cardPlay, card);
        }
    }
}
