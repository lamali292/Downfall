using BaseLib.Abstracts;
using Downfall.DownfallCode.Saves;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using Snecko.SneckoCode.Cards;

namespace Snecko.SneckoCode.Core;

public class SneckoModel() : CustomSingletonModel(true, true)
{
    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        if (oldPileType == PileType.None && card.Pile?.Type == PileType.Deck &&
            card is SneckoCardModel { Gift: { } gift }) await SneckoCmd.GetGift(card.Owner, gift);
    }


    private static void SetSneckoPools(Player player, IEnumerable<CardPoolModel> pools)
    {
        var pool = DownfallSaveManager.GetPlayerData(player).SneckoPools;
        pool.Clear();
        pool.AddRange(pools.Select(e => e.Id));
    }

    private static IEnumerable<CardPoolModel> GetSneckoPools(Player player)
    {
        return DownfallSaveManager.GetPlayerData(player).SneckoPools.Select(ModelDb.GetById<CardPoolModel>);
    }

    private static IEnumerable<CardModel> GetSneckoCards(Player player)
    {
        return GetSneckoPools(player).SelectMany(e => e.AllCards);
    }

    public static IEnumerable<CardModel> GetRewardSneckoCards(Player player)
    {
        return CardFactory.FilterForPlayerCount(player.RunState,
            CardFactory.FilterForCombat(GetSneckoCards(player)));
    }

    public static IEnumerable<CardModel> GetCombatSneckoCards(Player player, int amount)
    {
        return CardFactory.GetDistinctForCombat(player,
            GetSneckoCards(player),
            amount,
            player.RunState.Rng.CombatCardGeneration);
    }


    public override async Task AfterActEntered()
    {
        var state = RunManager.Instance.State;
        if (state is not { CurrentActIndex: 0 }) return;

        var sneckos = state.Players.Where(e => e.Character is Snecko).ToList();
        var choiceIds = sneckos.ToDictionary(
            snecko => snecko,
            snecko => Enumerable.Range(0, 3)
                .Select(_ => RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(snecko))
                .ToArray()
        );
        var tasks = sneckos.Select(async snecko =>
        {
            var pools = await SneckoPoolSelection.DoOffclassSelection(snecko, state, choiceIds[snecko]);
            return (snecko, pools);
        });
        var results = await Task.WhenAll(tasks);
        foreach (var (snecko, pools) in results)
            SetSneckoPools(snecko, pools);
    }
}