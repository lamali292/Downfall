using BaseLib.Patches.Content;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Piles;
using Downfall.DownfallCode.Commands;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Collector.CollectorCode.Core;

public class CollectorCmd
{
    public static async Task<CardModel?> Pyre(PlayerChoiceContext ctx, CardModel card)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1, 1);
        var pyred = (await CardSelectCmd.FromHand(ctx, card.Owner, prefs, e => e != card, card)).FirstOrDefault();
        if (pyred == null || card.CombatState == null) return pyred;
        await CardCmd.Exhaust(ctx, pyred);
        await CollectorHook.OnPyre(card.CombatState, ctx, card, pyred);
        return pyred;
    }


    public static async Task<CardPileAddResult> DrawCollected(PlayerChoiceContext ctx, Player player)
    {
        CollectorMainFile.Logger.Info($"DrawCollected: PileType = {CollectorPile.Collected}");
        CollectorMainFile.Logger.Info(
            $"Is registered: {CustomPiles.CustomPileProviders.ContainsKey(CollectorPile.Collected)}");
        if (player.Creature.CombatState == null) return default;
        return await DownfallCardCmd.DrawFromCustomPile(ctx, player, CollectorPile.Collected);
    }

    public static async Task<IReadOnlyList<CardPileAddResult>> DrawCollected(PlayerChoiceContext ctx, Player player,
        int amount)
    {
        if (player.Creature.CombatState == null) return [];
        return await DownfallCardCmd.DrawFromCustomPile(ctx, player, CollectorPile.Collected, amount);
    }


    public static async Task<Creature> SummonTorchhead(
        PlayerChoiceContext ctx,
        Player summoner,
        int hp,
        AbstractModel? source)
    {
        return await Summon<TorchheadMonsterModel>(ctx, summoner, hp, source);
    }


    public static Creature? Torchhead(Player summoner)
    {
        return GainPet<TorchheadMonsterModel>(summoner);
    }


    private static Creature? GainPet<T>(Player summoner) where T : MonsterModel
    {
        var combatState = summoner.Creature.CombatState;
        ArgumentNullException.ThrowIfNull(combatState);
        ArgumentNullException.ThrowIfNull(summoner.PlayerCombatState);
        return combatState.Allies.FirstOrDefault(c => c.Monster is T && c.PetOwner == summoner);
    }

    private static async Task<Creature> Summon<T>(
        PlayerChoiceContext ctx,
        Player summoner,
        int hp,
        AbstractModel? source) where T : MonsterModel
    {
        var combatState = summoner.Creature.CombatState;
        ArgumentNullException.ThrowIfNull(combatState);
        ArgumentNullException.ThrowIfNull(summoner.PlayerCombatState);
        var existing = combatState.Allies.FirstOrDefault(c => c.Monster is T && c.PetOwner == summoner);

        var isReviving = existing is { IsAlive: false };

        if (existing is { IsAlive: true })
        {
            await CreatureCmd.GainMaxHp(existing, hp);
            return existing;
        }

        if (isReviving)
        {
            summoner.PlayerCombatState.AddPetInternal(existing!);
        }
        else
        {
            existing = await PlayerCmd.AddPet<T>(summoner);
            var node = NCombatRoom.Instance?.GetCreatureNode(existing);
            var playerNode = NCombatRoom.Instance?.GetCreatureNode(summoner.Creature);

            if (node != null && source is CardModel && playerNode != null)
            {
                node.Position = playerNode.Position + new Vector2(250f, -75f);
                node.Modulate = Colors.Transparent;
                node.CreateTween()
                    .TweenProperty(node, "modulate", Colors.White, 0.35)
                    .SetDelay(0.1);
            }

            await PowerCmd.Apply<DieForYouPower>(ctx, existing, 1M, null, null);
            node?.TrackBlockStatus(summoner.Creature);
            node?.ToggleIsInteractable(true);
        }

        ArgumentNullException.ThrowIfNull(existing);
        await CreatureCmd.SetMaxHp(existing, hp);
        await CreatureCmd.Heal(existing, hp, isReviving);

        return existing;
    }
}