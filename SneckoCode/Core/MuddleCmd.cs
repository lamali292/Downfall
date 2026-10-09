using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Random;

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
        await Muddle(ctx, cards, lowerOnly);
    }

    public static async Task Muddle(PlayerChoiceContext ctx, IEnumerable<CardModel> cards,
        bool lowerOnly = false)
    {
        foreach (var cardModel in cards) await Muddle(ctx, cardModel, lowerOnly);
    }

    private static int RollMuddleCost(int maxCost, int currentCost, Rng rng)
    {
        if (maxCost <= 0)
            return 0;
        if (currentCost > maxCost)
            return rng.NextInt(maxCost + 1);
        var roll = rng.NextInt(maxCost);
        return roll >= currentCost ? roll + 1 : roll;
    }

    private const int MaxMuddleCost = 3;

    private static async Task<int> GetMaxMuddleCost(ICombatState cs, PlayerChoiceContext ctx, CardModel card)
    {
        var a = HookUtils.Modify<IMaxMuddleCost, int>(cs, MaxMuddleCost,
            (m, i) => m.ModifyMaxMuddleCost(card, i), out var modifiers);
        await HookUtils.AfterModifying(cs, modifiers, m => m.AfterModifyingMaxMuddleCost(ctx, card));
        return a;
    }
    
    private static async Task<bool> ShouldPermanentMuddle(ICombatState cs, PlayerChoiceContext ctx, CardModel card)
    {
        var permanentMuddle = HookUtils.Any<IShouldPermanentMuddleListener>(
            cs, e => e.ShouldPermanentMuddle(card), out var matches);
        await HookUtils.AfterModifying(cs, matches, e => e.AfterShouldPermanentMuddle(ctx, card));
        return permanentMuddle;
    }

    private static Task AfterCardMuddled(ICombatState cs, PlayerChoiceContext ctx, CardModel card )
    {
        return HookUtils.Dispatch<IAfterCardMuddled>(cs, m => m.AfterCardMuddled(ctx, card));
    }


    public static async Task<CardModel?> Muddle(PlayerChoiceContext ctx, CardModel card,
        bool lowerOnly = false)
    {
        var cs = card.Owner.Creature.CombatState;
        if (cs == null) return null;
        if (!CanMuddle(card))
            return null;
        
        var maxCost = await GetMaxMuddleCost(cs, ctx, card);
        var currentCost = card.EnergyCost.GetWithModifiers(CostModifiers.All);
        if (lowerOnly)
        {
            maxCost = Math.Min(maxCost, currentCost-1);
        }
        var newCost = RollMuddleCost(maxCost, currentCost, card.Owner.RunState.Rng.CombatEnergyCosts);


        var permanentMuddle = await ShouldPermanentMuddle(cs, ctx, card);
        if (permanentMuddle)
            card.EnergyCost.SetThisCombat(newCost);
        else
            card.EnergyCost.SetThisTurnOrUntilPlayed(newCost);

        NCard.FindOnTable(card)?.PlayRandomizeCostAnim();

        await AfterCardMuddled(cs, ctx, card);
        return card;
    }



    private static bool CanMuddle(CardModel card)
    {
        return !card.Keywords.Contains(CardKeyword.Unplayable) && !card.EnergyCost.CostsX;
    }
}

internal interface IMaxMuddleCost
{
    int ModifyMaxMuddleCost(CardModel card, int i);
    Task AfterModifyingMaxMuddleCost(PlayerChoiceContext choiceContext, CardModel card);
}

public interface IAfterCardMuddled
{
    Task AfterCardMuddled(PlayerChoiceContext choiceContext, CardModel cardModel);
}

public interface IShouldPermanentMuddleListener
{
    bool ShouldPermanentMuddle(CardModel card);
    Task AfterShouldPermanentMuddle(PlayerChoiceContext choiceContext, CardModel card);
}
