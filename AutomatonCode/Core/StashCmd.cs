using Automaton.AutomatonCode.Extensions;
using Automaton.AutomatonCode.Piles;
using Automaton.AutomatonCode.Vfx;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Core;

public class StashCmd
{
    public const int MaxStashSize = 5;

    public static LocString StashSelectionPrompt => new("card_selection", "AUTOMATON-TO_STASH");

    public static LocString FULL_STASH => new("combat_messages", "FULL_STASH");

    private static int RemainingSpace(Player player)
    {
        return Math.Max(0, MaxStashSize - player.GetStash().Count);
    }

    public static async Task StashUpTo(PlayerChoiceContext ctx, Player player, int amount, AbstractModel source)
    {
        var prefs = new CardSelectorPrefs(StashSelectionPrompt, 0, amount);
        var cards = await CardSelectCmd.FromHand(ctx, player, prefs, null, source);
        await Stash(player, cards);
    }

    public static async Task StashFromHand(CardModel source, PlayerChoiceContext ctx)
    {
        var amount = source.DynamicVars["Stash"].IntValue;
        var prefs = new CardSelectorPrefs(StashSelectionPrompt, amount);
        var cards = await CardSelectCmd.FromHand(ctx, source.Owner, prefs, null, source);
        await Stash(source.Owner, cards);
    }

    public static async Task StashFromDraw(CardModel source, PlayerChoiceContext ctx)
    {
        var amount = source.DynamicVars["Stash"].IntValue;
        var prefs = new CardSelectorPrefs(StashSelectionPrompt, amount);
        var cards = await CardSelectCmd.FromCombatPile(ctx, PileType.Draw.GetPile(source.Owner), source.Owner, prefs);
        await Stash(source.Owner, cards);
    }

    public static async Task Stash<TCard>(Player player, int amount = 1)
        where TCard : CardModel
    {
        NStashDisplay.EnsureFor(player);
        var toStash = Math.Min(amount, RemainingSpace(player));

        if (toStash > 0)
            await DownfallCardCmd.GiveCards<TCard>(player, StashPile.Stash, toStash);

        var overflow = amount - toStash;
        if (overflow > 0)
        {
            if (LocalContext.IsMe(player)) ThinkCmd.Play(FULL_STASH, player.Creature);
            await DownfallCardCmd.GiveCards<TCard>(player, PileType.Discard, overflow);
        }
    }

    public static async Task Stash(CardModel card)
    {
        NStashDisplay.EnsureFor(card.Owner);
        if (RemainingSpace(card.Owner) > 0)
        {
            await CardPileCmd.Add(card, StashPile.Stash);
        }
        else
        {
            if (LocalContext.IsMe(card.Owner)) ThinkCmd.Play(FULL_STASH, card.Owner.Creature);
            await CardPileCmd.Add(card, PileType.Discard);
        }
    }

    public static async Task Stash(Player player, IEnumerable<CardModel> cards)
    {
        var list = cards.ToList();
        if (list.Count == 0)
            return;

        NStashDisplay.EnsureFor(player);
        var space = RemainingSpace(player);
        var toStash = list.Take(space).ToList();
        var overflow = list.Skip(space).ToList();

        if (toStash.Count > 0)
            await CardPileCmd.Add(toStash, StashPile.Stash);

        if (overflow.Count > 0)
        {
            if (LocalContext.IsMe(player)) ThinkCmd.Play(FULL_STASH, player.Creature);
            await CardPileCmd.Add(overflow, PileType.Discard);
        }
    }


    public static async Task DrawFromStash(CardModel card, PlayerChoiceContext ctx, ICombatState combatState)
    {
        var cards = card.Owner.GetStash();
        var n = card.DynamicVars.Cards.IntValue;
        await CardPileCmd.Add(cards.Take(n).ToList(), PileType.Hand);
        await Hook.AfterCardDrawn(combatState, ctx, card, false);
    }

    public static async Task<IReadOnlyList<CardPileAddResult>> DrawFromStash(Player player, int n = 1)
    {
        var cards = player.GetStash();
        return await CardPileCmd.Add(cards.Take(n).ToList(), PileType.Hand);
    }
}