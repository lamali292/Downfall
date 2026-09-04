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
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        foreach (var players in participants.Select(e => e.Player).OfType<Player>())
        {
            var torchhead = players.Torchhead?.Monster as TorchheadMonsterModel;
            var target = players.Creature.CombatState?.HittableEnemies.OrderBy(e => e.CurrentHp).FirstOrDefault();
            if (target == null || torchhead == null) continue;
            await DamageCmd.Attack(5)
                .FromTorchhead(torchhead)
                .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3")
                .Targeting(target).Execute(choiceContext);
        }

    }


    private readonly List<MonsterModel> _defeatedEnemies = [];

    public override Task BeforeCombatStart()
    {
        _defeatedEnemies.Clear();
        return Task.CompletedTask;
    }

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented,
        float deathAnimLength)
    {
        if (creature is { IsEnemy: true, Monster: not null })
            _defeatedEnemies.Add(creature.Monster);
        return Task.CompletedTask;
    }


    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (room.RoomType is not (RoomType.Elite or RoomType.Boss)) return Task.CompletedTask;
        var cards = ModelDb.CardPool<CollectibleCardPool>().AllCards.OfType<ICollectible>();
        var enemyCards = _defeatedEnemies
            .Where(e => cards.Any(c => c.GetMonsterModel().Id == e.Id))
            .ToList();
        
        foreach (var player in room.CombatState.Players.Where(p => p.Character is Collector))
        {
            foreach (var cardModel in enemyCards)
                room.AddExtraReward(player, new CollectibleReward(cardModel.Id, player));
        }
        return Task.CompletedTask;
    }
}