using BaseLib.Abstracts;
using Hermit.HermitCode.Cards.Rare;
using Hermit.HermitCode.Events;
using Hermit.HermitCode.History;
using Hermit.HermitCode.Patches;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Core;

public static class HermitCmd
{
    /// <summary>
    ///     The one Dead On question: is this card Dead On right now, for the play it is part of?
    ///     <para>
    ///         <c>IShouldTriggerDeadOn</c> sources are always re-asked live (their answer can change
    ///         between replay instances of the same card). Hand-position Dead On is read live while
    ///         the card is in hand, and from <see cref="DeadOnPatch" />'s pre-play snapshot once it
    ///         sits in the Play pile (hand position is gone by then but never changes during a play).
    ///     </para>
    /// </summary>
    public static bool IsDeadOn(CardModel card)
    {
        if (card.CombatState != null && HermitHook.ShouldTriggerDeadOn(card.CombatState, card))
            return true;
        return card.Pile?.Type switch
        {
            PileType.Hand => IsDeadOnByHandPosition(card),
            PileType.Play => DeadOnPatch.WasPlayedDeadOn(card),
            _ => false
        };
    }

    // Pure hand-position check, no hooks. Only DeadOnPatch (for its snapshot) and IsDeadOn use it.
    internal static bool IsDeadOnByHandPosition(CardModel card)
    {
        var hand = PileType.Hand.GetPile(card.Owner).Cards.ToList();
        return HandGeometry.IsCenter(hand, card);
    }

    public static bool IsAdjacentToCurse(CardModel card)
    {
        return (card.Pile?.Type == PileType.Hand && IsAdjacentToCurseInCurrentHandState(card)) ||
               (card.Pile?.Type == PileType.Play && WasThisPlayedAdjacentToCurse(card));
    }

    private static bool WasThisPlayedAdjacentToCurse(CardModel card)
    {
        return DeadOnPatch.WasPlayedAdjacentToCurse(card);
    }


    public static bool IsAdjacentToCurseInCurrentHandState(CardModel cardModel)
    {
        var hand = PileType.Hand.GetPile(cardModel.Owner).Cards.ToList();
        return HandGeometry.IsAdjacentToMatch(hand, cardModel, c => c.Type == CardType.Curse);
    }

    public static bool HasActiveDeadOnEffect(CardModel card)
    {
        return IsDeadOn(card) && HasDeadOn(card);
    }

    public static bool HasDeadOn(CardModel card)
    {
        return card is IHasDeadOnEffect ||
               CardModifier.Modifiers(card).OfType<DeadOnReplay>().Any();
    }


    public static async Task TriggerDeadOnEffect(PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay)
    {
        var combatState = card.CombatState!;

        var modify = HermitHook.ModifyDeadOnCount(combatState, 1, card, out var modifiers);
        var hasEffect = card is IHasDeadOnEffect;
        var hasReplayModifier = CardModifier.Modifiers(card).OfType<DeadOnReplay>().Any();
        if (!hasEffect && !hasReplayModifier) return;
        if (card is IHasDeadOnEffect cardModel)
            for (var i = 0; i < modify; i++)
                await cardModel.DeadOnEffect(ctx, cardPlay);
        await HermitHook.AfterModifyingDeadOnCount(combatState, ctx, card, modifiers);
        var entry = new DeadOnEntry(cardPlay, card.Owner.Creature, combatState.RoundNumber,
            card.Owner.Creature.Side, CombatManager.Instance.History, combatState.Players);
        CombatManager.Instance.History.Add(combatState, entry);
        await HermitHook.AfterDeadOnTrigger(combatState, ctx, card, cardPlay);
    }
}