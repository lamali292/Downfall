using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Events;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Core;

public class SneckoModel() : CustomSingletonModel(HookType.Run)
{
    public static IEnumerable<CharacterModel> GetSneckoCharacterModels(Player player)
    {
        // Each ISneckoPoolSupplier (SneckoChoice, PrismaticSnecko, ...) contributes the character
        // pool(s) it grants - usually one, but Prismatic Snecko contributes every character at once.
        return MyHookUtils.Collect<ISneckoPoolSupplier, IEnumerable<CharacterModel>>(null,
                supplier => supplier.AddSneckoChars(), MyHookUtils.HookScope.Run, player.RunState)
            .SelectMany(c => c)
            .Distinct();
    }


    // Card pools of the characters Snecko currently borrows from; every other character's pool when it borrows from none.
    private static IEnumerable<CardPoolModel> GetSneckoPools(Player player)
    {
        var pools = GetSneckoCharacterModels(player).Select(e => e.CardPool).ToList();
        return pools.Count > 0
            ? pools
            : ModelDb.AllCharacters.Where(e => e != player.Character).Select(c => c.CardPool).ToList();
    }

    // Unfiltered: the combat and reward pipelines each apply their own filtering (multiplayer constraint, rarity, ...).
    private static IEnumerable<CardModel> GetSneckoCards(Player player)
    {
        return GetSneckoPools(player).SelectMany(e => e.AllCards);
    }

    // Same-rarity replacement for `original` from the borrowed pools, or null when there is none. Built here rather than
    // via CardTransformation's option list: the game's transformation rules always narrow those options to
    // Common/Uncommon/Rare, which would leave Basic/Ancient/Event cards without a replacement.
    public static CardModel? CreateTransformationReplacement(Player player, CardModel original, Rng rng)
    {
        var options = GetSneckoPools(player)
            .SelectMany(p => p.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint))
            .Where(c => c.Id != original.Id && c.CanBeGeneratedInCombat && c.Rarity == original.Rarity)
            .ToList();
        var template = rng.NextItem(options);
        return template == null ? null : original.CardScope?.CreateCard(template, player);
    }

    // Reward pipeline (rarity odds, upgrade roll, multiplayer constraint, reward-modifying relics) over the borrowed pools.
    public static CardCreationOptions GetRewardOptions(Player player, Func<CardModel, bool>? filter = null)
    {
        return CardCreationOptions.ForNonCombatWithDefaultOdds(GetSneckoPools(player).ToList(), filter);
    }

    public static IEnumerable<CardModel> CreateRewardSneckoCards(Player player, int amount,
        Func<CardModel, bool>? filter = null)
    {
        return CardFactory.CreateForReward(player, amount, GetRewardOptions(player, filter)).Select(e => e.Card);
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


    public override Task AfterRoomEntered(AbstractRoom room)
    {
        var state = RunManager.Instance.DebugOnlyGetState()!;
        if (state.Act.ActNumber() > 1 || state.ActFloor > 1) return Task.CompletedTask;
        SneckoPoolSelection.RunActEntry(RunManager.Instance.DebugOnlyGetState()!);
        return Task.CompletedTask;
    }
}