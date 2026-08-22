using Automaton.AutomatonCode.Events;
using Automaton.AutomatonCode.Extensions;
using Automaton.AutomatonCode.Piles;
using Automaton.AutomatonCode.Vfx;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
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
        return Math.Max(0, MaxStashSize - player.StashPile.Count);
    }

    private static void NotifyFullStash(Player player)
    {
        if (LocalContext.IsMe(player)) ThinkCmd.Play(FULL_STASH, player.Creature);
    }

    // ---- the one and only stash flow ----------------------------------------
    // Splits `cards` by remaining space, stashes what fits, discards the rest,
    // and fires the "full stash" ping on overflow. `place` is the primitive that
    // actually puts cards into a pile (differs for live vs generated cards).
    private static async Task Run(
        PlayerChoiceContext ctx,
        Player player,
        List<CardModel> cards,
        Func<List<CardModel>, PileType, Task<IReadOnlyList<CardPileAddResult>>> place)
    {
        if (cards.Count == 0)
            return;

        NStashDisplay.EnsureFor(player);

        var space = RemainingSpace(player);
        var toStash = cards.Take(space).ToList();
        var overflow = cards.Skip(space).ToList();

        if (toStash.Count > 0)
        {
            var a = await place(toStash, StashPile.Stash);
            CardCmd.PreviewCardPileAdd(a, 0.2f);
        }
          

        if (overflow.Count > 0)
        {
            NotifyFullStash(player);
            var a = await place(overflow, PileType.Discard);
            CardCmd.PreviewCardPileAdd(a, 0.2f);
        }
        await AutomatonHook.AfterCardsStashed(player.Creature.CombatState, ctx, player, toStash, overflow);
    }

    // Placement primitive for cards already registered in combat.
    private static Task<IReadOnlyList<CardPileAddResult>> PlaceExisting(List<CardModel> cards, PileType target)
        => CardPileCmd.Add(cards, target, skipVisuals: true);

    // ---- entry points -------------------------------------------------------

    public static Task Stash( PlayerChoiceContext ctx, CardModel card)
        => Run(ctx, card.Owner, [card], PlaceExisting);

    public static Task Stash( PlayerChoiceContext ctx, Player player, IEnumerable<CardModel> cards)
        => Run(ctx, player, cards.ToList(), PlaceExisting);

    public static Task Stash<TCard>( PlayerChoiceContext ctx, Player player, int amount = 1)
        where TCard : CardModel
    {
        var cards = BuildCards<TCard>(player, amount);
        return Run(ctx, player, cards, (list, target)
            => CardPileCmd.AddGeneratedCardsToCombat(list, target, player));
    }

    // Creation loop lifted out of DownfallCardCmd.GiveCards.
    private static List<CardModel> BuildCards<TCard>(Player player, int amount, bool upgraded = false)
        where TCard : CardModel
    {
        var list = new List<CardModel>();
        if (amount <= 0) return list;

        var model = ModelDb.Card<TCard>();
        for (var i = 0; i < amount; i++)
        {
            var card = (TCard)player.Creature.CombatState!.CreateCard(model, player);
            if (upgraded) card.UpgradeInternal();
            list.Add(card);
        }
        return list;
    }

    // ---- selection helpers (unchanged) --------------------------------------

    public static async Task StashUpTo(PlayerChoiceContext ctx, Player player, int amount, AbstractModel source)
    {
        var prefs = new CardSelectorPrefs(StashSelectionPrompt, 0, amount);
        var cards = await CardSelectCmd.FromHand(ctx, player, prefs, null, source);
        await Stash(ctx, player, cards);
    }

    public static async Task StashFromHand(CardModel source, PlayerChoiceContext ctx)
    {
        var amount = source.DynamicVars["Stash"].IntValue;
        var prefs = new CardSelectorPrefs(StashSelectionPrompt, amount);
        var cards = await CardSelectCmd.FromHand(ctx, source.Owner, prefs, null, source);
        await Stash(ctx, source.Owner, cards);
    }

    public static async Task StashFromDraw(CardModel source, PlayerChoiceContext ctx)
    {
        var amount = source.DynamicVars["Stash"].IntValue;
        var prefs = new CardSelectorPrefs(StashSelectionPrompt, amount);
        var cards = await CardSelectCmd.FromCombatPile(ctx, PileType.Draw.GetPile(source.Owner), source.Owner, prefs);
        await Stash(ctx, source.Owner, cards);
    }

    // ---- draw-from-stash (unchanged) ----------------------------------------

    public static Task<IReadOnlyList<CardPileAddResult>> DrawFromStash(PlayerChoiceContext ctx, CardModel card)
    {
        return DrawFromStash(ctx, card.Owner, card.DynamicVars.Cards.IntValue);
    }

    public static async Task<IReadOnlyList<CardPileAddResult>> DrawFromStash(PlayerChoiceContext ctx, Player player, int n = 1)
    {
        var cards = player.StashPile;
        var result = await CardPileCmd.Add(cards.Take(n).ToList(), PileType.Hand);
        foreach (var cardPileAddResult in result)
        {
            var drawn = cardPileAddResult.cardAdded;
            var combatState = drawn.CombatState!;
            CombatManager.Instance.History.Add(combatState,
                new CardDrawnEntry(drawn, combatState.RoundNumber, combatState.CurrentSide, false,
                    CombatManager.Instance.History, combatState.Players));
            await Hook.AfterCardDrawn(combatState, ctx, drawn, false);
        }
        return result;
    }
}