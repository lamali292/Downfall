using BaseLib.Abstracts;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Rewards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Collector.CollectorCode.Core;

public class CollectorModel() : CustomSingletonModel(HookType.Combat)
{
    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side, IEnumerable<Creature> participants)
    {
        foreach (var players in participants.Select(e => e.Player).OfType<Player>())
        {
            await CollectorCmd.TorchheadAttack(ctx, players, 5);
        }

    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (room.RoomType is not (RoomType.Elite or RoomType.Boss)) return Task.CompletedTask;
        var existsCard = ModelDb.CardPool<CollectibleCardPool>().AllCards.Any(c => c is ICollectible col && col.GetEncounterModel().Id == room.Encounter.Id);
        if (!existsCard) return Task.CompletedTask;
        foreach (var player in room.CombatState.Players.Where(p => p.Character is Collector))
        {
            room.AddExtraReward(player, new CollectibleReward(room.Encounter.Id, player));
        }
        return Task.CompletedTask;
    }
}