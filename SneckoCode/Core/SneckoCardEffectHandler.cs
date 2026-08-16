using System.Runtime.CompilerServices;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Events;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Core;

public static class SneckoCardEffectHandler
{
    
    private static readonly SpireField<CardPlay, bool> OverflowSnapshot = new(() => false);
    
    public static Task<bool> DoBeforeOnPlayInternal(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        OverflowSnapshot.Set(cardPlay, SneckoCmd.OverflowActive(card));
        return Task.FromResult(true);
    }
    
    public static async Task DoAfterOnPlayInternal(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (card is IHasOverflowEffect overflow
            && card.Keywords.Contains(SneckoKeywords.Overflow)
            && OverflowSnapshot[cardPlay])
        {
            await overflow.OverflowEffect(ctx, cardPlay);
            await SneckoHook.AfterOverflowEffect(card.Owner, cardPlay, card);
        }
    }
}