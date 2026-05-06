using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Runs;
using Snecko.SneckoCode.Events;

namespace Snecko.SneckoCode.Core;

public static class SneckoCmd
{
    public static Task MuddleHandCards(PlayerChoiceContext ctx, CardModel card, bool lowerOnly = false)
    {
        var amount = card.DynamicVars["Muddle"].IntValue;
        return MuddleHandCards(ctx, card, amount, lowerOnly);
    }

    private static LocString MuddleSelectionPrompt => new("card_selection", "TO_MUDDLE");
    
    private static async Task MuddleHandCards(PlayerChoiceContext ctx, CardModel card, int amount, bool lowerOnly = false)
    {
        var prefs = new CardSelectorPrefs(MuddleSelectionPrompt, amount);
        var cards = await CardSelectCmd.FromHand(ctx, card.Owner, prefs, c => c != card && CanMuddle(c), card);
        await Muddle(ctx, cards, card, lowerOnly);
    }
    
    public static async Task Muddle(PlayerChoiceContext ctx, IEnumerable<CardModel> cards, AbstractModel? source, bool lowerOnly = false)
    {
        foreach (var cardModel in cards)
        {
            await Muddle(ctx, cardModel, source, lowerOnly);
        }
    }

    public static async Task Muddle(PlayerChoiceContext ctx, CardModel card, AbstractModel? source = null, bool lowerOnly = false)
    {
        card.EnergyCost.SetThisTurn(NextEnergyCost(card, lowerOnly));
        NCard.FindOnTable(card)?.PlayRandomizeCostAnim();
        await SneckoHook.AfterCardMuddled(card.CombatState!, ctx, card, source);
    }
    
    private static int NextEnergyCost(CardModel card, bool lowerOnly = false)
    {
        var current = card.EnergyCost.GetResolved();
        if (current == 0 && lowerOnly) return 0;
        var max = lowerOnly ? current : 4;
        var rng = card.Owner.RunState.Rng.CombatEnergyCosts;
        int next;
        do { next = rng.NextInt(max); } while (next == current);
        return next;
    }

    private static bool CanMuddle(CardModel card)
    {
        return !card.Keywords.Contains(CardKeyword.Unplayable) && !card.EnergyCost.CostsX;
    }

    public static bool OverflowActive(Player player, bool cardInHand = false) =>
        player.PlayerCombatState is { Hand.Cards.Count: var count } && count > (cardInHand ? 5 : 4);


    public static bool IsOffclass(CardModel card, CardModel other) =>
        other.Pool != card.Pool;
    public static bool IsOffclass(Player player, CardModel other) =>
        other.Pool != player.Character.CardPool;

    public static bool IsDebuff(CardModel card)
    {
        return card.DynamicVars.Values.Any(IsDebuffPowerVar) && card.TargetType is not (TargetType.Self or TargetType.None);
    }

    private static bool IsDebuffPowerVar(DynamicVar v)
    {
        var t = v.GetType();
        if (!t.IsGenericType || t.GetGenericTypeDefinition() != typeof(PowerVar<>))
            return false;
        
        var method = typeof(ModelDb).GetMethod("Power")?.MakeGenericMethod(t.GetGenericArguments()[0]);
        var power = method?.Invoke(null, null) as PowerModel;
        return power?.Type == PowerType.Debuff && v.BaseValue > 0;
    }

    public static async Task GetGift(Player player, Gift gift, int amount = 3)
    {
        var sneckoCards =  CardFactory.FilterForPlayerCount(player.RunState,
            CardFactory.FilterForCombat(SneckoModel.GetSneckoCards(player)));
        var cards = sneckoCards.Where(gift.Matches)
            .TakeRandom(amount, player.RunState.Rng.CombatCardGeneration).Select(e=>e.ToMutable()).ToList();
        foreach (var cardChoice in cards)
        {
            player.RunState.AddCard(cardChoice, player);
            if (gift.IsUpgraded)
            {
                cardChoice.UpgradeInternal();
            }
        }

        var card = await CardSelectCmd.FromChooseACardScreen(new BlockingPlayerChoiceContext(), cards, player);
        if  (card == null) return;
        var a = await CardPileCmd.Add(card, PileType.Deck);
        CardCmd.PreviewCardPileAdd(a, 0);

        if (gift.Gold is > 0)
        {
            await PlayerCmd.GainGold(gift.Gold.Value, player);
        }
        
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