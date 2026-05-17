using Automaton.AutomatonCode.Cards;
using Automaton.AutomatonCode.Cards.Rare;
using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Displays;
using Automaton.AutomatonCode.Events;
using Automaton.AutomatonCode.Interfaces;
using Automaton.AutomatonCode.Piles;
using BaseLib.Patches.Content;
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
    public static int GetSequenceCount(Player creature)
    {
        return GetEncodePile(creature)?.Cards.Count ?? 0;
    }

    public static IReadOnlyList<CardModel> GetSequence(Player creature)
    {
        return GetEncodePile(creature)?.Cards ?? [];
    }

    public static AutomatonPile? GetEncodePile(Player creature)
    {
        return CustomPiles.GetCustomPile(creature.PlayerCombatState, AutomatonPile.FunctionSequence) as
            AutomatonPile;
    }

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
        var pile = GetEncodePile(creature);
        if (pile == null) return;
        var isMe = LocalContext.IsMe(creature);
        if (isMe && card.Pile?.Type == PileType.Hand)
        {
            var hand = NCombatRoom.Instance?.Ui.Hand;
            hand?.Remove(card);
        }

        if (isMe) await AutomatonDisplay.AnimateCardToSequence(card, pile, creature);
        await CardPileCmd.Add(card, pile, skipVisuals: isMe);
        if (isMe) AutomatonDisplay.Refresh(creature);
        await AutomatonHook.OnCardEncoded(card.CombatState!, ctx, card, cardPlay);
        if (pile.Cards.Count >= GetMax(creature))
            await CompileFunctionCard(creature, ctx, cardPlay);
    }


    public static async Task CompileFunctionCard(
        Player creature,
        PlayerChoiceContext ctx,
        CardPlay cardPlay)
    {
        var pile = GetEncodePile(creature);
        if (pile == null) return;
        await Cmd.Wait(0.3f);
        var combatState = creature.Creature.CombatState;
        if (combatState == null) return;
        var snapshot = pile.Cards.OfType<AutomatonCardModel>().ToList();
        pile.Clear(true);
        if (LocalContext.IsMe(creature))
            AutomatonDisplay.Refresh(creature);
        var functionCard = CreateFunctionCardFromSnapshot(cardPlay, snapshot, combatState);
        for (var i = 0; i < snapshot.Count; i++)
        {
            var card = snapshot[i];
            var compileContext = new CompileContext(i, snapshot.Count);
            switch (card)
            {
                case ICompilableError compileErrorCard when !card.SuppressCompileError:
                    await compileErrorCard.OnCompileError(ctx, functionCard, cardPlay, compileContext, true);
                    break;
                case ICompilable compileCard:
                    await compileCard.OnCompile(ctx, functionCard, cardPlay, compileContext, true);
                    break;
            }
        }

        await AutomatonHook.OnCompile(ctx, combatState, snapshot, functionCard, cardPlay);
        await CardPileCmd.AddGeneratedCardToCombat(functionCard, PileType.Hand, creature);
        //if (result.success)
        //    CardCmd.PreviewCardPileAdd(result, 0.7f);
    }

    private static FunctionCard CreateFunctionCardFromSnapshot(CardPlay cardPlay, List<AutomatonCardModel> snapshot,
        ICombatState combatState)
    {
        var functionCard = combatState.CreateCard<FunctionCard>(cardPlay.Card.Owner);
        if (snapshot.Any(c => c is FullRelease))
        {
            functionCard.SetCardType(CardType.Power);
            functionCard.SetTargetType(TargetType.None);
        }
        else if (snapshot.Any(c => c.TargetType == TargetType.AnyEnemy || c.Type == CardType.Attack))
        {
            functionCard.SetCardType(CardType.Attack);
            functionCard.SetTargetType(TargetType.AnyEnemy);
        }
        else
        {
            functionCard.SetCardType(CardType.Skill);
            functionCard.SetTargetType(TargetType.Self);
        }

        functionCard.SetSourceCards(snapshot);
        return functionCard;
    }


    public static async Task MoveFromSequenceToHand(CardModel card, Player creature)
    {
        await CardPileCmd.Add(card, PileType.Hand);

        if (LocalContext.IsMe(creature))
            AutomatonDisplay.Refresh(creature);
    }

    public static async Task MoveFromSequenceToHand(IEnumerable<CardModel> cards, Player creature)
    {
        await CardPileCmd.Add(cards, PileType.Hand);
        if (LocalContext.IsMe(creature))
            AutomatonDisplay.Refresh(creature);
    }
}