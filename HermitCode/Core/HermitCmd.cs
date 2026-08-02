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
    public static bool IsDeadOnInCurrentHandState(CardModel card)
    {
        if (card.CombatState == null) return false;
        if (HermitHook.ShouldTriggerDeadOn(card.CombatState, card))
            return true;

        var handCards = PileType.Hand.GetPile(card.Owner).Cards.ToList();
        var cardIndex = handCards.IndexOf(card);
        if (cardIndex == -1)
            return false;

        var handSize = handCards.Count;
        if (handSize % 2 == 0)
            return cardIndex == handSize / 2 - 1 || cardIndex == handSize / 2;
        return cardIndex == handSize / 2;
    }

    public static bool IsInDeadOnState(CardModel card) => (card.Pile?.Type == PileType.Hand && IsDeadOnInCurrentHandState(card)) ||
                                                          (card.Pile?.Type == PileType.Play && WasThisPlayedDeadOn(card));


    private static bool WasThisPlayedDeadOn(CardModel card) => DeadOnPatch.LastPlayed == card && DeadOnPatch.LastWasDeadOn;

    public static bool IsAdjacentToCurse(CardModel card) => (card.Pile?.Type == PileType.Hand && IsAdjacentToCurseInCurrentHandState(card)) ||
                                                            (card.Pile?.Type == PileType.Play && WasThisPlayedAdjacentToCurse(card));

    private static bool WasThisPlayedAdjacentToCurse(CardModel card) =>
        DeadOnPatch.LastPlayed == card && DeadOnPatch.LastWasAdjacentToCurse;

    
    public static bool IsAdjacentToCurseInCurrentHandState(CardModel cardModel)
    {
        var hand = PileType.Hand.GetPile(cardModel.Owner).Cards.ToList();
        var idx = hand.IndexOf(cardModel);
        if (idx == -1) return false;

        var leftIsCurse = idx > 0 && hand[idx - 1].Type == CardType.Curse;
        var rightIsCurse = idx < hand.Count - 1 && hand[idx + 1].Type == CardType.Curse;
        return leftIsCurse || rightIsCurse;
    }
    
    public static bool HasActiveDeadOnEffect(CardModel card)
    {
        return IsInDeadOnState(card) && HasDeadOn(card);
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