using BaseLib.Abstracts;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Collector.Token;
using Downfall.Code.Commands;
using Downfall.Code.Events;
using Downfall.Code.Piles;
using Downfall.Code.Rewards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Downfall.Code.Core.Collector;

public class CollectorModel() : CustomSingletonModel(true, false)
{
    private readonly List<MonsterModel> _defeatedEnemies = [];
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (DownfallHook.PreventCollectedDraw(combatState, player)) return;
        await CollectorCmd.DrawCollected(ctx, player);
    }

    public override Task BeforeCombatStart()
    {
        _defeatedEnemies.Clear();

        var combatState = CombatManager.Instance.DebugOnlyGetState();
        if (combatState == null) return Task.CompletedTask;

        foreach (var player in combatState.Players.Where(p => p.Character is Character.Collector))
        {
            var pile = CollectorPile.Collected.GetPile(player);
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
        foreach (var player in room.CombatState.Players.Where(p => p.Character is Character.Collector))
        {
            if (essenceAmount > 0) room.AddExtraReward(player, new EssenceReward(essenceAmount, player));
            foreach (var cardModel in enemyCards)
                room.AddExtraReward(player, new CollectibleReward(cardModel.ToMutable(), player));
        }

        return Task.CompletedTask;
    }
}

[HarmonyPatch(typeof(RunState), nameof(RunState.CreateForNewRun))]
internal class PatchNewRun
{
    [HarmonyPostfix]
    private static void GiveStartingEssence(RunState __result)
    {
        foreach (var player in __result.Players)
        {
            EssenceModel.ClearEssence(player);
            CollectiblesModel.ClearCollectibles(player);
            if (player.Character is Character.Collector)
                EssenceModel.AddEssence(player, 5);
        }
    }
}