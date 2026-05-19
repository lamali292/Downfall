using Hermit.HermitCode.Events;
using Hermit.HermitCode.History;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Core;

public static class HermitCmd
{
    public static bool IsDeadOnInCurrentHandState(CardModel card)
    {
        if (HermitHook.ShouldTriggerDeadOn(card.CombatState!, card))
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
    
    
    public static async Task TriggerDeadOnEffect(PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay)
    {
        var combatState = card.CombatState!;
        
        var modify = HermitHook.ModifyDeadOnCount(combatState, 1, card, out var modifiers);
        await HermitHook.AfterModifyingDeadOnCount(combatState, ctx, card, modifiers);
        
        if (card is not IHasDeadOnEffect cardModel) return;
        for (var i = 0; i <  modify; i++)
            await cardModel.DeadOnEffect(ctx, cardPlay);
        var entry = new DeadOnEntry(cardPlay, card.Owner.Creature, combatState.RoundNumber,  card.Owner.Creature.Side,
            CombatManager.Instance.History);
        CombatManager.Instance.History.Add(combatState, entry);
        await HermitHook.AfterDeadOnTrigger(combatState, ctx, card, cardPlay);
    }
}