using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using Snecko.SneckoCode.Events;
using Snecko.SneckoCode.History;

namespace Snecko.SneckoCode.Core;

public static class MuddleCmd
{
    private static LocString MuddleSelectionPrompt => new("card_selection", "TO_MUDDLE");

    public static Task MuddleHandCards(PlayerChoiceContext ctx, CardModel card, bool lowerOnly = false)
    {
        var amount = card.DynamicVars["Muddle"].IntValue;
        return MuddleHandCards(ctx, card, amount, lowerOnly);
    }

    private static async Task MuddleHandCards(PlayerChoiceContext ctx, CardModel card, int amount,
        bool lowerOnly = false)
    {
        var prefs = new CardSelectorPrefs(MuddleSelectionPrompt, amount);
        var cards = await CardSelectCmd.FromHand(ctx, card.Owner, prefs, c => c != card && CanMuddle(c), card);
        await Muddle(ctx, cards, card, lowerOnly);
    }

    public static async Task Muddle(PlayerChoiceContext ctx, IEnumerable<CardModel> cards, AbstractModel? source,
        bool lowerOnly = false)
    {
        foreach (var cardModel in cards) await Muddle(ctx, cardModel, source, lowerOnly);
    }

    public static async Task Muddle(PlayerChoiceContext ctx, CardModel card, AbstractModel? source = null,
        bool lowerOnly = false)
    {
        var combatState = card.CombatState;
        if (combatState == null) return;
        card.EnergyCost.SetThisTurn(NextEnergyCost(card, lowerOnly));
        NCard.FindOnTable(card)?.PlayRandomizeCostAnim();
        await SneckoHook.AfterCardMuddled(combatState, ctx, card, source);
        var entry = new MuddleEntry(card, card.Owner.Creature, combatState.RoundNumber, card.Owner.Creature.Side,
            CombatManager.Instance.History, combatState.Players);
        CombatManager.Instance.History.Add(combatState, entry);
    }

    private static int NextEnergyCost(CardModel card, bool lowerOnly = false)
    {
        var current = card.EnergyCost.GetResolved();
        if (current == 0 && lowerOnly) return 0;
        const int normalMax = 4;
        var max = lowerOnly ? Math.Min(normalMax, current) : normalMax;
        var rng = card.Owner.RunState.Rng.CombatEnergyCosts;

        var valid = Enumerable.Range(0, max)
            .Where(cost => cost != current && SneckoHook.ShouldAllowMuddleCost(card.CombatState!, card, cost))
            .ToList();

        if (valid.Count == 0)
            valid = Enumerable.Range(0, max).ToList();

        return valid[rng.NextInt(valid.Count)];
    }

    private static bool CanMuddle(CardModel card)
    {
        return !card.Keywords.Contains(CardKeyword.Unplayable) && !card.EnergyCost.CostsX;
    }
}
