using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class PrismaticTorch : CollectorRelicModel
{
    public PrismaticTorch() : base(RelicRarity.Starter)
    {
        WithKindle(10);
    }

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext ctx,
        ICombatState combatState)
    {
        if (player != Owner || Owner.PlayerCombatState is not { TurnNumber: 1 }) return;
        await CollectorCmd.Kindle(ctx, this);
        Flash();
    }
    /*
    
    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (room.RoomType is not (RoomType.Elite or RoomType.Boss)) return Task.CompletedTask;
        var existsCard = ModelDb.CardPool<CollectibleCardPool>().AllCards.Any(c => c is ICollectible col && col.GetEncounterModel().Id == room.Encounter.Id);
        if (!existsCard) return Task.CompletedTask;
        foreach (var player in room.CombatState.Players.Where(p => p.Character is Core.Collector))
        {
            room.AddExtraReward(player, new CollectibleReward(room.Encounter.Id, player, true));
        }
        return Task.CompletedTask;
    }*/

    public override bool TryModifyCardRewardOptions(Player player, List<CardCreationResult> cardRewardOptions, CardCreationOptions creationOptions)
    {
        if (Owner != player
            || creationOptions.Source != CardCreationSource.Encounter
            || !creationOptions.Flags.HasFlag(CardCreationFlags.IsCardReward)
            || !creationOptions.Flags.HasFlag(CardCreationFlags.IsFromCombat))
            return false;
        
        var room = player.RunState.CurrentRoom as CombatRoom;
        if (room?.RoomType is not (RoomType.Elite or RoomType.Boss))
            return false;

        var encounterId = room.Encounter.Id;

        var model = ModelDb.CardPool<CollectibleCardPool>().AllCards
            .FirstOrDefault(c => c is ICollectible g && g.GetEncounterModel().Id == encounterId);
        if (model is null)
            return false;

        var card = player.RunState.CreateCard(model, player);
        CardCmd.Upgrade(card);
        var result = new CardCreationResult(card);
        result.ModifyCard(card, this);
        cardRewardOptions.Add(result);
        return true;
        
    }
    
    /*
    public override Task AfterModifyingCardRewardOptions()
    {
        Flash();
        return Task.CompletedTask;
    }*/
}