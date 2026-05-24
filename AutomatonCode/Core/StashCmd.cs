using Automaton.AutomatonCode.Extensions;
using Automaton.AutomatonCode.Piles;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Core;

public class StashCmd
{
    public static LocString StashSelectionPrompt => new("card_selection", "AUTOMATON-TO_STASH");

    public static async Task StashUpTo(PlayerChoiceContext ctx, Player player, int amount, AbstractModel source)
    {
        var prefs = new CardSelectorPrefs(StashSelectionPrompt, 0, amount);
        var cards = await CardSelectCmd.FromHand(ctx, player, prefs, null, source);
        await Stash(cards);
    }

    public static async Task StashFromHand(CardModel source, PlayerChoiceContext ctx)
    {
        var amount = source.DynamicVars["Stash"].IntValue;
        var prefs = new CardSelectorPrefs(StashSelectionPrompt, amount);
        var cards = await CardSelectCmd.FromHand(ctx, source.Owner, prefs, null, source);
        await Stash(cards);
    }

    public static async Task StashFromDraw(CardModel source, PlayerChoiceContext ctx)
    {
        var amount = source.DynamicVars["Stash"].IntValue;
        var prefs = new CardSelectorPrefs(StashSelectionPrompt, amount);
        var cards = await CardSelectCmd.FromCombatPile(ctx, PileType.Draw.GetPile(source.Owner), source.Owner, prefs);
        await Stash(cards);
    }


    public static async Task Stash<TCard>(Player player, int amount = 1)
        where TCard : CardModel
    {
        await DownfallCardCmd.GiveCards<TCard>(player, StashPile.Stash, amount);
    }

    public static async Task Stash(CardModel card)
    {
        await CardPileCmd.Add(card, StashPile.Stash);
    }

    public static async Task Stash(IEnumerable<CardModel> cards)
    {
        await CardPileCmd.Add(cards, StashPile.Stash);
    }


    public static async Task DrawFromStash(CardModel card)
    {
        var cards = card.Owner.GetStash();
        var n = card.DynamicVars.Cards.IntValue;
        await CardPileCmd.Add(cards.Take(n).ToList(), PileType.Hand);
    }

    public static async Task DrawFromStash(Player player, int n = 1)
    {
        var cards = player.GetStash();
        await CardPileCmd.Add(cards.Take(n).ToList(), PileType.Hand);
    }
}