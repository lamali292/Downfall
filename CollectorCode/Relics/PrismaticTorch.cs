using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Rewards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

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
    }
    
}