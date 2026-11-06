using Awakened.AwakenedCode.Cards.Uncommon;
using Awakened.AwakenedCode.Events;
using Awakened.AwakenedCode.History;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;

namespace Awakened.AwakenedCode.Core;

/// <summary>
/// Chant: a card with the <see cref="CustomEnums.AwakenedKeyword.Chant"/> keyword gets its effect
/// played again (once, or more with Rising Chorus) when the previous card played was a Power, or once
/// it has ever chanted before. Each chant card calls <see cref="Chant"/> itself from its own
/// OnPlayInternal, passing its own effect as a delegate - there is no shared dispatch interface.
/// </summary>
public static class ChantCmd
{
    private static readonly SpireField<CardModel, bool> HasCardChanted = new(() => false);

    public static bool HasChanted(CardModel card)
    {
        return HasCardChanted.Get(card);
    }

    /// <summary>Marks a card as having chanted. Exposed publicly only so tests can force the state
    /// without playing a Power card first; production code should never need to call this directly.</summary>
    public static void SetChanted(CardModel card)
    {
        HasCardChanted.Set(card, true);
    }

    public static bool WasLastCardPlayedPower(CardModel card)
    {
        if (!CombatManager.Instance.IsInProgress) return false;
        var lastCardEntry = CombatManager.Instance.History.CardPlaysStarted
            .LastOrDefault(e =>
                e.CardPlay.Card.Owner == card.Owner &&
                e.CardPlay.Card != card);

        if (lastCardEntry == null) return false;
        return lastCardEntry.CardPlay.Card.Type == CardType.Power;
    }

    private static bool WasLastCardPlayedPower(CardPlay cardPlay)
    {
        if (!CombatManager.Instance.IsInProgress) return false;
        var lastCardEntry = CombatManager.Instance.History.CardPlaysStarted
            .LastOrDefault(e =>
                e.CardPlay.Card.Owner == cardPlay.Card.Owner &&
                e.CardPlay != cardPlay);

        if (lastCardEntry == null) return false;

        return lastCardEntry.CardPlay.Card.Type == CardType.Power;
    }

    public static async Task Chant(CardPlay cardPlay, Func<Task> effect)
    {
        if (WasLastCardPlayedPower(cardPlay) || HasChanted(cardPlay.Card))
            await ChantInternal(cardPlay, effect);
    }

    private static async Task ChantInternal(CardPlay cardPlay, Func<Task> effect)
    {
        var card = cardPlay.Card;
        var combatState = card.CombatState;
        if (combatState == null) return;
        var firstTime = !HasChanted(card);
        if (firstTime && card is not Caw && TestMode.IsOff)
        {
            TalkCmd.Play(new LocString("monsters", "DAMP_CULTIST.moves.INCANTATION.banter"), card.Owner.Creature,
                VfxColor.Blue);
            SfxCmd.Play("event:/sfx/characters/awakened-awakened/chant");
        }

        SetChanted(card);
        var entry = new ChantEntry(cardPlay, combatState.RoundNumber,
            combatState.CurrentSide, CombatManager.Instance.History, combatState.Players);
        CombatManager.Instance.History.Add(combatState, entry);
        var repeatCount = AwakenedHook.ModifyChantRepeatCount(combatState, card, cardPlay, 1, out var modifiers);
        await AwakenedHook.AfterModifyingChantRepeatCount(combatState, card, cardPlay, modifiers);

        for (var i = 0; i < repeatCount; i++)
        {
            await effect.Invoke();
        }
    }
}
