using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Piles;
using Collector.CollectorCode.Rewards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
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
        if (CollectorHook.PreventCollectedDraw(combatState, player) || player.Character is not Collector) return;
        await CollectorCmd.DrawCollected(ctx, player);
    }

    public override Task BeforeCombatStart()
    {
        _defeatedEnemies.Clear();

        var combatState = CombatManager.Instance.DebugOnlyGetState();
        if (combatState == null) return Task.CompletedTask;

        foreach (var player in combatState.Players.Where(p => p.Character is Collector))
        {
            var pile = CustomPiles.GetCustomPile(player.PlayerCombatState, CollectorPile.Collected);
            if (pile == null) continue;
            pile.Clear();

            var collectibles = CollectiblesModel.GetCollectibles(player);

            // shuffle using player rng
            collectibles.UnstableShuffle(combatState.RunState.Rng.Shuffle);

            foreach (var mutable in collectibles.Select(card => (CardModel)card.MutableClone()))
            {
                combatState.AddCard(mutable, player);
                pile.AddInternal(mutable);
                pile.InvokeCardAddFinished();
            }
        }

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
            .Select(e => cards.FirstOrDefault(c => c.GetMonsterModel().Id == e.Id))
            .OfType<CardModel>()
            .ToList();
        var essenceAmount = room.RoomType switch
        {
            RoomType.Monster => 1,
            RoomType.Elite => 2,
            RoomType.Boss => 3,
            _ => 0
        };
        foreach (var player in room.CombatState.Players.Where(p => p.Character is Collector))
        {
            if (essenceAmount > 0) room.AddExtraReward(player, new EssenceReward(essenceAmount, player));
            foreach (var cardModel in enemyCards)
                room.AddExtraReward(player, new CollectibleReward(cardModel.ToMutable(), player));
        }

        return Task.CompletedTask;
    }
}