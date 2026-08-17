using BaseLib.Abstracts;
using Downfall.DownfallCode.Events;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Core;

public class SneckoModel() : CustomSingletonModel(HookType.Run)
{
    private static IEnumerable<CardPoolModel> GetSneckoPools(Player player)
    {
        return MyHookUtils.Collect<ISneckoPoolSupplier, CardPoolModel>(null, supplier => supplier.AddSneckoPool(),
            MyHookUtils.HookScope.Run, player.RunState);
    }

    public static IEnumerable<CardModel> GetSneckoCards(Player player)
    {
        var cards = GetSneckoPools(player)
            .SelectMany(e => CardFactory.FilterForPlayerCount(player.RunState, e.AllCards));
        if (cards != null && cards.Any()) return cards;
        return ModelDb.AllCharacters
            .Where(e => e != player.Character)
            .ToList().Select(c => c.CardPool).ToList().SelectMany(e => e.AllCards);
    }

    public static IEnumerable<CardModel> GetRewardSneckoCards(Player player, Func<CardModel, bool>? filter = null)
    {
        var cards = GetSneckoCards(player);
        if (filter is not null) cards = cards.Where(filter);
        return CardFactory.FilterForPlayerCount(player.RunState,
            CardFactory.FilterForCombat(cards));
    }

    public static IEnumerable<CardModel> GetCombatSneckoCards(Player player, int amount, Player? forPlayer = null,
        Func<CardModel, bool>? filter = null)
    {
        forPlayer ??= player;
        var cards = GetSneckoCards(player);
        if (filter is not null) cards = cards.Where(filter);
        return CardFactory.GetDistinctForCombat(forPlayer,
            cards,
            amount,
            player.RunState.Rng.CombatCardGeneration);
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        if (card.Pile?.Type == PileType.Deck &&
            card is IHasGift { Gift: { } gift })
            await SneckoCmd.GetGift(card.Owner, gift);
    }
}





