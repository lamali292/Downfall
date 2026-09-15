using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using Snecko.SneckoCode.Events;
using Snecko.SneckoCode.History;

namespace Snecko.SneckoCode.Core;

public static class SneckoCmd
{
    private static readonly Dictionary<Type, PowerModel?> PowerCache = new();
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

    public static bool OverflowActive(CardModel card)
    {
        return card.Owner.Hand.Count(e => e != card) >= 5;
    }


    public static bool IsDebuff(CardModel card)
    {
        return card.DynamicVars.Values.Any(IsDebuffPowerVar) &&
               card.TargetType is not (TargetType.Self or TargetType.AllAllies or TargetType.AnyPlayer
                   or TargetType.Osty or TargetType.AnyAlly);
    }

    private static bool IsDebuffPowerVar(DynamicVar v)
    {
        var t = v.GetType();
        if (!t.IsGenericType || t.GetGenericTypeDefinition() != typeof(PowerVar<>))
            return false;

        if (!PowerCache.TryGetValue(t, out var power))
            PowerCache[t] = power = typeof(ModelDb)
                .GetMethod(nameof(ModelDb.Power))
                ?.MakeGenericMethod(t.GetGenericArguments()[0])
                .Invoke(null, null) as PowerModel;
        return power?.GetTypeForAmount(v.BaseValue) == PowerType.Debuff;
    }

    public static async Task GetGift(Player player, Gift gift, int amount = 3)
    {
        var sneckoCards = SneckoModel.GetRewardSneckoCards(player);
        var cards = sneckoCards.Where(gift.Matches)
            .TakeRandom(amount, player.RunState.Rng.CombatCardGeneration)
            .Select(e => e.ToMutable())
            .ToList();
        foreach (var cardChoice in cards)
        {
            player.RunState.AddCard(cardChoice, player);
            if (gift.IsUpgraded) cardChoice.UpgradeInternal();
        }

        // Gift is documented as "get a card reward", so offer it through the actual reward system:
        // this gets Silver Crucible/DingyRug/etc. modification (RewardsSet.Populate() calls into
        // CardReward.Populate()), proper MP sync (PlayerChoiceSynchronizer, same as before, just via
        // the engine's own tested implementation), free TestMode support, and - crucially - the
        // reward-set stack, so spamming multiple Gift-granting purchases queues extra reward screens
        // instead of racing to show several at once.
        var rerollOptions = CardCreationOptions.ForNonCombatWithDefaultOdds(Array.Empty<CardPoolModel>());
        var cardReward = new CardReward(cards, CardCreationSource.Other, player, rerollOptions);
        await RewardsCmd.OfferCustom(player, [cardReward]);

        if (cardReward.SuccessfullySelected && gift.Gold is > 0) await PlayerCmd.GainGold(gift.Gold.Value, player);
    }
}

public readonly struct Gift
{
    public CardRarity? Rarity { get; init; }
    public CardType? Type { get; init; }
    public bool IsDebuff { get; init; }
    public bool IsStrike { get; init; }
    public int? MinCost { get; init; }
    public int? Gold { get; init; }
    public bool IsUpgraded { get; init; }

    public bool Matches(CardModel card)
    {
        if (Rarity.HasValue && card.Rarity != Rarity.Value) return false;
        if (Type.HasValue && card.Type != Type.Value) return false;
        if (IsDebuff && !SneckoCmd.IsDebuff(card)) return false;
        if (IsStrike && !card.Tags.Contains(CardTag.Strike)) return false;
        return !MinCost.HasValue || card.EnergyCost.Canonical >= MinCost.Value;
    }
}