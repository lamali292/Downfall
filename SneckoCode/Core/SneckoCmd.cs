using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace Snecko.SneckoCode.Core;

public static class SneckoCmd
{
    private static readonly Dictionary<Type, PowerModel?> PowerCache = new();

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
        return power?.GetDownfallTypeForAmount(v.BaseValue) == PowerType.Debuff;
    }

    public static async Task GetGift(Player player, Gift gift, int amount = 3)
    {
        var options = SneckoModel.GetRewardOptions(player, gift.Matches);
        var cards = CardFactory.CreateForReward(player, amount, options).Select(e => e.Card).ToList();
        if (gift.IsUpgraded)
            foreach (var card in cards)
                card.UpgradeInternal();
        
        var cardReward = new CardReward(cards, CardCreationSource.Other, player, options);
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