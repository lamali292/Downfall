using Automaton.AutomatonCode.Cards.Rare;
using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Displays;
using Automaton.AutomatonCode.Enchantments;
using Automaton.AutomatonCode.Events;
using Automaton.AutomatonCode.Interfaces;
using Automaton.AutomatonCode.Piles;
using Automaton.AutomatonCode.Relics;
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

    public static async Task EncodeCard(
        CardModel card,
        PlayerChoiceContext ctx)
    {
        var creature = card.Owner;
        var pile = EncodePile.FunctionSequence.GetPile(creature);
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
        AutomatonDisplay.Refresh(creature);

        if (pile.Cards.Count >= GetMax(creature))
            await CompileFunctionCard(creature, ctx);

        await AutomatonHook.OnCardEncoded(creature.Creature.CombatState!, ctx, card);
    }


    private static async Task CompileFunctionCard(
        Player player,
        PlayerChoiceContext ctx)
    {
        var pile = EncodePile.FunctionSequence.GetPile(player);
        await Cmd.Wait(0.3f);
        var combatState = player.Creature.CombatState;
        if (combatState == null) return;
        var snapshot = pile.Cards.ToList();
        pile.Clear(true);

        AutomatonDisplay.Refresh(player);

        var functionCard = combatState.CreateCard<FunctionCard>(player);
        functionCard.SetSourceCards(snapshot);
        ApplyFunctionCardType(functionCard, snapshot);
        functionCard = AutomatonHook.ModifyCompiledFunction(combatState, functionCard, player, out var modifiers);
        await AutomatonHook.AfterModifyCompiledFunction(combatState, modifiers, player, functionCard);
        var result = await CardPileCmd.AddGeneratedCardToCombat(functionCard, PileType.Hand, player);
        await AutomatonHook.AfterCompilingFunction(ctx, combatState, player, result);
    }

    public static void ApplyFunctionCardType(FunctionCard card, IEnumerable<CardModel> snapshot)
    {
        var list = snapshot.ToList();

        if (list.Any(c => c is { TargetType: TargetType.AnyEnemy }))
            card.SetTargetType(TargetType.AnyEnemy);
        else if (list.Any(c => c is { TargetType: TargetType.AllEnemies }))
            card.SetTargetType(TargetType.AllEnemies);
        else
            card.SetTargetType(TargetType.Self);

        if (list.Any(c => c is FullRelease))
            card.SetCardType(CardType.Power);
        else if (list.Any(c => c is { Type: CardType.Attack }))
            card.SetCardType(CardType.Attack);
        else
            card.SetCardType(CardType.Skill);

        if (list.Any(c => c.Rarity == CardRarity.Ancient))
            card.SetCardRarity(CardRarity.Ancient);
        else if (list.Any(c => c.Rarity == CardRarity.Rare))
            card.SetCardRarity(CardRarity.Rare);
        else if (list.Any(c => c.Rarity == CardRarity.Uncommon))
            card.SetCardRarity(CardRarity.Uncommon);
        else
            card.SetCardRarity(CardRarity.Common);
    }

    public static bool IsEncodable(CardModel card)
    {
        return card is IEncodable || card.Enchantment is Encoding;
    }
}