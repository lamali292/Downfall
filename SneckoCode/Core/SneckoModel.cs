using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Events;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Runs;
using Snecko.SneckoCode.Cards;
using Snecko.SneckoCode.Interfaces;
using Snecko.SneckoCode.Relics;

namespace Snecko.SneckoCode.Core;

public class SneckoModel() : CustomSingletonModel(HookType.Run)
{
    
    public static IEnumerable<CharacterModel> GetSneckoCharacterModels(Player player)
    {
        return MyHookUtils.Collect<ISneckoPoolSupplier, CharacterModel>(null, supplier => supplier.AddSneckoChar(),
            MyHookUtils.HookScope.Run, player.RunState);
    }

    
    private static IEnumerable<CardPoolModel> GetSneckoPools(Player player)
    {
        return GetSneckoCharacterModels(player).Select(e => e.CardPool);
    }

    public static IEnumerable<CardModel> GetSneckoCards(Player player)
    {
        var cards = GetSneckoPools(player)
            .SelectMany(e => CardFactory.FilterForPlayerCount(player.RunState, e.AllCards)).ToList();
        if (cards.Count > 0) return cards;
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

    public override async Task AfterActEntered()
    {
        await SneckoPoolSelection.RunActEntry(RunManager.Instance.State!);
    }

 

}