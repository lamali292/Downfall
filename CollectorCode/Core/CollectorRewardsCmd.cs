using BaseLib.Extensions;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Powers;
using Collector.CollectorCode.Rewards;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Compatibility;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Collector.CollectorCode.Core;

public static class CollectorRewardsCmd
{
    
    
    

    
    public static bool TryAddCollectiblesReward(RelicModel relic, Player player, List<CardCreationResult> cardRewardOptions, CardCreationOptions creationOptions, Action<CardModel>? action = null)
    {
        if (creationOptions.Source != CardCreationSource.Encounter
            || !creationOptions.Flags.HasFlag(CardCreationFlags.IsCardReward)
            )
            return false;
        // maybe add || !creationOptions.Flags.HasFlag(CardCreationFlags.IsFromCombat) back
        if (player.RunState.CurrentRoom is not CombatRoom { RoomType: RoomType.Elite or RoomType.Boss } room)
            return false;

        var encounterId = room.Encounter.Id;
        var pool = ModelDb.CardPool<CollectibleCardPool>().AllCards.ToList();
        // get our collectibles
        var model = pool.FirstOrDefault(c => c is ICollectible g && g.GetEncounterModel()?.Id == encounterId);
        // fallback to other mods
        model ??= GetCardForModdedEnemy(player, encounterId);
        // final fallback. pick random elite or boss with the same act number.
        model ??= player.RunState.Rng.Niche.NextItem(pool
                .Where(c => c is ICollectible g && 
                            (g.Act()?.ActNumber() ?? -1) == room.Act.ActNumber() &&
                            g.RoomType() == room.RoomType)
        );
        if (model is null) return false;
        var card = player.RunState.CreateCard(model, player);
        action?.Invoke(card);
        var result = new CardCreationResult(card);
        result.ModifyCard(card, relic);
        cardRewardOptions.Add(result);
        return true;
    }

    private static CardModel? GetCardForModdedEnemy(Player player, ModelId encounterId)
    {
        var value = ModdedEncounterCollectibles.TryGetCardEntries(encounterId.Entry);
        if (value == null) return null;
        if (value.Count > 1) value.StableShuffle(player.PlayerRng.Rewards);
        if (value.Count == 0) return null;
        var id = new ModelId("CARD", value[0]);
        var card = ModelDb.GetByIdOrNull<CardModel>(id);
        return card;
    }
    
    
    
}