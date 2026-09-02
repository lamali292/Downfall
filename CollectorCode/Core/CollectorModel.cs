using BaseLib.Abstracts;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Rewards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Collector.CollectorCode.Core;

public class CollectorModel() : CustomSingletonModel(HookType.Combat)
{
    private readonly List<MonsterModel> _defeatedEnemies = [];
    public override bool ShouldReceiveCombatHooks => true;
    

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