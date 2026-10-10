using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Random;

namespace Snecko.SneckoCode.Core;

/// <summary>
///     The Muddle rule itself: roll a new cost, run every mod's Muddle listeners (see
///     <see cref="MuddleListenerRegistry" />). Downfall's Snecko and IntoTheSpireverse each have an identical copy of
///     this file; keep them in sync. Mod-specific entry points (card selection, prompts, ...) live in
///     <c>MuddleCmd</c>.
/// </summary>
public static class MuddleCore
{
    private const int MaxMuddleCost = 3;

    public static bool CanMuddle(CardModel card)
    {
        return !card.Keywords.Contains(CardKeyword.Unplayable) && !card.EnergyCost.CostsX;
    }

    public static async Task<CardModel?> Muddle(PlayerChoiceContext ctx, CardModel card, bool lowerOnly = false)
    {
        var cs = card.Owner.Creature.CombatState;
        if (cs == null) return null;
        if (!CanMuddle(card))
            return null;

        var listeners = MuddleListenerRegistry.Collect(cs);

        var maxCost = await GetMaxMuddleCost(listeners, ctx, card);
        var currentCost = card.EnergyCost.GetWithModifiers(CostModifiers.All);
        if (lowerOnly)
            maxCost = Math.Min(maxCost, currentCost - 1);
        var newCost = RollMuddleCost(maxCost, currentCost, card.Owner.RunState.Rng.CombatEnergyCosts);

        if (await ShouldPermanentMuddle(listeners, ctx, card))
            card.EnergyCost.SetThisCombat(newCost);
        else
            card.EnergyCost.SetThisTurnOrUntilPlayed(newCost);

        NCard.FindOnTable(card)?.PlayRandomizeCostAnim();

        await AfterCardMuddled(listeners, ctx, card);
        return card;
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

    private static async Task<int> GetMaxMuddleCost(List<MuddleListener> listeners, PlayerChoiceContext ctx,
        CardModel card)
    {
        var maxCost = MaxMuddleCost;
        List<MuddleListener> modifiers = [];
        foreach (var listener in listeners.Where(l => l.ModifyMaxCost != null))
        {
            var modified = listener.ModifyMaxCost!(card, maxCost);
            if (modified == maxCost) continue;
            maxCost = modified;
            modifiers.Add(listener);
        }

        foreach (var modifier in modifiers)
        {
            await modifier.AfterModifyMaxCost!(ctx, card);
            modifier.Model.InvokeExecutionFinished();
        }

        return maxCost;
    }

    private static async Task<bool> ShouldPermanentMuddle(List<MuddleListener> listeners, PlayerChoiceContext ctx,
        CardModel card)
    {
        // No short-circuit: every listener gets asked, and every one that said yes gets its "After" call.
        var matches = listeners.Where(l => l.ShouldPermanent != null && l.ShouldPermanent(card)).ToList();
        foreach (var match in matches)
        {
            await match.AfterShouldPermanent!(ctx, card);
            match.Model.InvokeExecutionFinished();
        }

        return matches.Count > 0;
    }

    private static async Task AfterCardMuddled(List<MuddleListener> listeners, PlayerChoiceContext ctx,
        CardModel card)
    {
        foreach (var listener in listeners.Where(l => l.AfterMuddled != null))
            await listener.AfterMuddled!(ctx, card);
    }
}

public interface IMaxMuddleCost
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
