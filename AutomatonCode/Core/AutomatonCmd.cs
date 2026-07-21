using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Events;
using Automaton.AutomatonCode.Interfaces;
using Automaton.AutomatonCode.Piles;
using Automaton.AutomatonCode.Relics;
using Automaton.AutomatonCode.Vfx;
using BaseLib.Patches.Content;
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
        return creature.GetRelic<ElectromagneticCoil>() == null ? 3 : 4;
    }

    public static async Task<FunctionCard?> EncodeCard(
        CardModel card,
        PlayerChoiceContext ctx)
    {
        var creature = card.Owner;
        var pile = CustomPiles.GetCustomPile(creature.PlayerCombatState, EncodePile.FunctionSequence);
        if (pile == null) return null;
        var isMe = LocalContext.IsMe(creature);


        if (isMe && card.Pile?.Type == PileType.Hand)
        {
            var hand = NCombatRoom.Instance?.Ui.Hand;
            hand?.Remove(card);
        }

        //if (isMe) await AutomatonDisplay.AnimateCardToSequence(card, pile, creature);
        await Cmd.Wait(0.2f);
        await CardPileCmd.Add(card, pile);
        await Cmd.Wait(0.2f);
        NSequenceDisplay.Refresh(creature);

        FunctionCard? functionCard = null;
        if (pile.Cards.Count >= GetMax(creature))
            functionCard = await CompileFunctionCard(creature, ctx);

        await AutomatonHook.OnCardEncoded(creature.Creature.CombatState!, ctx, card);
        return functionCard;
    }


    private static async Task<FunctionCard?> CompileFunctionCard(
        Player player,
        PlayerChoiceContext ctx)
    {
        var pile = CustomPiles.GetCustomPile(player.PlayerCombatState, EncodePile.FunctionSequence);
        if (pile == null) return null;
        await Cmd.Wait(0.3f);
        var combatState = player.Creature.CombatState;
        if (combatState == null) return null;
        var snapshot = pile.Cards.ToList();
        pile.Clear(true);

        NSequenceDisplay.Refresh(player);
        foreach (var cardModel in snapshot)
            if (cardModel is ICompilable compilable)
                await compilable.OnCompile(ctx);

        var functionCard = combatState.CreateCard<FunctionCard>(player);
        functionCard.SetSourceCards(snapshot);
        functionCard = AutomatonHook.ModifyCompiledFunction(combatState, functionCard, player, out var modifiers);
        await AutomatonHook.AfterModifyCompiledFunction(combatState, modifiers, player, functionCard);
        var result = await CardPileCmd.AddGeneratedCardToCombat(functionCard, PileType.Hand, player);
        await AutomatonHook.AfterCompilingFunction(ctx, combatState, player, result);
        return functionCard;
    }


    public static bool IsEncodable(CardModel card)
    {
        return card is IEncodable { CanPlayerEncode: true };
    }

    public static async Task EncodeEffect(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (card is not IEncodable encodable) return;
        foreach (var encodableEncoding in encodable.Encodings)
            await encodableEncoding.OnPlay(card, ctx, cardPlay.Target, cardPlay);
    }
}