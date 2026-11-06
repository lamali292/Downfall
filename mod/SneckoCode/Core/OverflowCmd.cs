using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Cards;
using Snecko.SneckoCode.Events;

namespace Snecko.SneckoCode.Core;

public static class OverflowCmd
{
    public static bool OverflowActive(CardModel card)
    {
        return card.Owner.Hand.Count(e => e != card) >= 5;
    }

    public static async Task Overflow(CardPlay cardPlay, Func<Task> effect)
    {
        if (!OverflowActive(cardPlay.Card)) return;
        await effect.Invoke();
        await SneckoHook.AfterOverflowEffect(cardPlay.Card.Owner, cardPlay, cardPlay.Card);
    }
    
    /// <summary>Runs <paramref name="effect"/> only if <paramref name="wasActive"/> - the result of an
    /// <see cref="OverflowActive"/> call the card itself made at the very top of its OnPlayInternal, before
    /// doing anything else. Overflow must be decided from the hand as it was when the card started
    /// resolving, not as it stands once the bonus effect would fire: another card/power reacting to this
    /// card's own effect (e.g. "draw a card on Attack") can change hand size mid-play, and that must not
    /// flip the decision for this play. The card still has to <see cref="SneckoCardModel.WithOverflow"/>
    /// itself for the glow/tip to show.</summary>
    public static async Task Overflow(bool wasActive, CardPlay cardPlay, Func<Task> effect)
    {
        if (!wasActive) return;
        await effect.Invoke();
        await SneckoHook.AfterOverflowEffect(cardPlay.Card.Owner, cardPlay, cardPlay.Card);
    }
}
