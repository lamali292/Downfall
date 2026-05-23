using Automaton.AutomatonCode.Cards.Rare;
using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Displays;
using Automaton.AutomatonCode.Events;
using Automaton.AutomatonCode.Piles;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Automaton.AutomatonCode.Core;

public static class AutomatonCmd
{
    

    public static int GetMax(Player creature)
    {
        return 3;
    }

    public static async Task EncodeCard(
        CardModel card,
        PlayerChoiceContext ctx,
        CardPlay cardPlay)
    {
        var creature = cardPlay.Card.Owner;
        var pile = EncodePile.FunctionSequence.GetPile(creature);
        var isMe = LocalContext.IsMe(creature);
        
        
        if (isMe && card.Pile?.Type == PileType.Hand)
        {
            var hand = NCombatRoom.Instance?.Ui.Hand;
            hand?.Remove(card);
        }

        //if (isMe) await AutomatonDisplay.AnimateCardToSequence(card, pile, creature);
        await CardPileCmd.Add(card, pile);
        if (isMe) AutomatonDisplay.Refresh(creature);
        
        await AutomatonHook.OnCardEncoded(card.CombatState!, ctx, card, cardPlay);
        if (pile.Cards.Count >= GetMax(creature))
            await CompileFunctionCard(creature, ctx, cardPlay);
    }


    private static async Task CompileFunctionCard(
        Player player,
        PlayerChoiceContext ctx,
        CardPlay cardPlay)
    {
        var pile = EncodePile.FunctionSequence.GetPile(player);
        await Cmd.Wait(0.3f);
        var combatState = player.Creature.CombatState;
        if (combatState == null) return;
        var snapshot = pile.Cards.ToList();
        pile.Clear(true);
        
        AutomatonDisplay.Refresh(player);
        
        var functionCard = combatState.CreateCard<FunctionCard>(cardPlay.Card.Owner);
        functionCard.SetSourceCards(snapshot);
        ApplyFunctionCardType(functionCard, snapshot);
        functionCard =  AutomatonHook.ModifyCompiledFunction(combatState, functionCard, player, out var modifiers);
        await AutomatonHook.AfterModifyCompiledFunction(combatState, modifiers, player, functionCard);
        var result = await CardPileCmd.AddGeneratedCardToCombat(functionCard, PileType.Hand, player);
        await AutomatonHook.AfterCompilingFunction(ctx, combatState, player, result, cardPlay);
    }

    public static void ApplyFunctionCardType(FunctionCard card, IEnumerable<CardModel> snapshot)
    {
        var list = snapshot.ToList();
        if (list.Any(c => c is FullRelease))
        {
            card.SetCardType(CardType.Power);
            card.SetTargetType(TargetType.None);
        }
        else if (list.Any(c => c is { TargetType: TargetType.AnyEnemy, Type: CardType.Attack }))
        {
            card.SetCardType(CardType.Attack);
            card.SetTargetType(TargetType.AnyEnemy);
        }
        else if (list.Any(c => c is { TargetType: TargetType.AllEnemies, Type: CardType.Attack }))
        {
            card.SetCardType(CardType.Attack);
            card.SetTargetType(TargetType.AllEnemies);
        }
        else if (list.Any(c => c.TargetType == TargetType.AnyEnemy))
        {
            card.SetCardType(CardType.Skill);
            card.SetTargetType(TargetType.AnyEnemy);
        }
        else
        {
            card.SetCardType(CardType.Skill);
            card.SetTargetType(TargetType.Self);
        }
    }
    
 
}