using BaseLib.Abstracts;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Rewards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Collector.CollectorCode.Core;

public class CollectorModel() : CustomSingletonModel(HookType.Combat)
{
    private readonly List<MonsterModel> _defeatedEnemies = [];
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        //throw new NotImplementedException();
    }

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