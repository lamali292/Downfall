using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Events;
using Automaton.AutomatonCode.Extensions;
using Automaton.AutomatonCode.Interfaces;
using Automaton.AutomatonCode.Piles;
using Automaton.AutomatonCode.Relics;
using Automaton.AutomatonCode.Vfx;
using BaseLib.Patches.Content;
using Downfall.DownfallCode.Commands;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Core;

public static class AutomatonCmd
{
    public static int GetMax(Player creature)
    {
        return creature.GetRelic<ElectromagneticCoil>() == null ? 3 : 4;
    }

    /// <summary>
    ///     All Encodable cards available to the player. Automaton players draw only from
    ///     their own pool; other characters draw Encodable cards from every pool.
    /// </summary>
    public static IEnumerable<CardModel> GetEncodableCards(Player player, int amount)
    {
        return DownfallCardCmd.GetSpecificCards<Automaton>(player, IsEncodable, amount);
    }


    public static async Task<FunctionCard?> EncodeCard<T>(
        Player player,
        PlayerChoiceContext ctx, Action<CardModel>? func = null) where T : CardModel
    {
        var card = player.Creature.CombatState!.CreateCard<T>(player);
        func?.Invoke(card);
        return await EncodeCard(card, ctx);
    }


    public static async Task<FunctionCard?> EncodeCard(
        CardModel card,
        PlayerChoiceContext ctx)
    {
        var player = card.Owner;
        if (LocalContext.IsMe(player))
            Callable.From(() => NEncodePile.RevealFor(player)).CallDeferred();
        await Cmd.Wait(0.2f);
        await CardPileCmd.Add(card, EncodePile.FunctionSequence);
        await Cmd.Wait(0.2f);
        EncodePile.FunctionSequence.GetPile(player).InvokeContentsChanged();
        //NSequenceDisplay.Refresh(creature);

        FunctionCard? functionCard = null;
        if (player.EncodePile.Count >= GetMax(player))
        {
            functionCard = await CompileFunctionCard(player, ctx);
        }


        await AutomatonHook.OnCardEncoded(player.Creature.CombatState!, ctx, card);
        return functionCard;
    }


    private static async Task<FunctionCard?> CompileFunctionCard(
        Player player,
        PlayerChoiceContext ctx)
    {
        var pile = CustomPiles.GetCustomPile(player.PlayerCombatState, EncodePile.FunctionSequence);
        if (pile == null) return null;
        await Cmd.Wait(0.5f);
        var combatState = player.Creature.CombatState;
        if (combatState == null) return null;
        var snapshot = pile.Cards.ToList();
        pile.Clear();
        
        //NSequenceDisplay.Refresh(player);
        foreach (var cardModel in snapshot)
            if (cardModel is ICompilable compilable)
                await compilable.OnCompile(ctx);

        var functionCard = combatState.CreateCard<FunctionCard>(player);
        functionCard.SetSourceCards(snapshot);
        functionCard = AutomatonHook.ModifyCompiledFunction(combatState, functionCard, player, out var modifiers);
        await AutomatonHook.AfterModifyCompiledFunction(combatState, modifiers, player, functionCard);
        var result = await CardPileCmd.AddGeneratedCardToCombat(functionCard, PileType.Hand, player); ;
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