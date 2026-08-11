using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Events;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Core;

public static class SneckoCardEffectHandler
{
    public static async Task DoAfterOnPlayInternal(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (card is IHasOverflowEffect { HandlesOverflowSelf: false } overflow
            && card.Keywords.Contains(SneckoKeywords.Overflow)
            && SneckoCmd.OverflowActive(card))
        {
            await overflow.OverflowEffect(ctx, cardPlay);
            await SneckoHook.AfterOverflowEffect(card.Owner, cardPlay, card);
        }
    }
}