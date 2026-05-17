using BaseLib.Extensions;
using Downfall.DownfallCode.Extensions;
using Downfall.DownfallCode.Powers;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Downfall.DownfallCode.Commands;

public class DownfallCmd
{
    public static Task GainTempHp(PlayerChoiceContext ctx, CardModel card)
    {
        return GainTempHp(ctx, card, card.DynamicVars["TempHP"].BaseValue);
    }

    public static Task GainTempHp(PlayerChoiceContext ctx, CardModel card, decimal tempHp)
    {
        return PowerCmd.Apply<TempHpPower>(ctx, card.Owner.Creature, tempHp, card.Owner.Creature,
            card);
    }

    public static Task GainTempHp(PlayerChoiceContext ctx, Creature creature, decimal tempHp)
    {
        return PowerCmd.Apply<TempHpPower>(ctx, creature, tempHp, creature,
            null);
    }


    public static int GetTempHpAmount(Creature creature)
    {
        return creature.GetPowerAmount<TempHpPower>();
    }


    public static async Task EnemyAttackPlayer(PlayerChoiceContext ctx, CardPlay cardPlay, CardModel card)
    {
        var monster = cardPlay.Target?.Monster;
        if (cardPlay.Target == null || monster == null) return;
        if (!cardPlay.Target.IsAlive) return;
        var player = card.Owner;
        var attacker = monster.Creature;
        await Cmd.Wait(0.5f);

        var enemyDamage = card.DynamicVars.EnemyDamage();
        var attack = DamageCmd.Attack(enemyDamage.BaseValue);
        attack.Attacker = attacker;
        attack._attackerAnimName = "Attack";
        attack._sourceType = AttackCommand.SourceType.Monster;
        await attack
            .Targeting(player.Creature)
            .WithHitFx("vfx/vfx_attack_slash", "event:/sfx/characters/silent/silent_attack")
            .Execute(ctx);
    }


    public static async Task Steal<T>(PlayerChoiceContext ctx, CardPlay cardPlay, CardModel card)
        where T : PowerModel
    {
        switch (card.TargetType)
        {
            case TargetType.AnyEnemy:
                if (cardPlay.Target == null) return;
                await Steal<T>(ctx, cardPlay.Target, card);
                break;
            case TargetType.AllEnemies:
                if (card.CombatState == null) return;
                await Steal<T>(ctx, card.CombatState.HittableEnemies, card);
                break;
            case TargetType.RandomEnemy:
                if (card.CombatState == null) return;
                await Steal<T>(ctx,
                    card.CombatState.HittableEnemies.TakeRandom(1, card.CombatState.RunState.Rng.CombatTargets), card);
                break;
        }
    }


    public static Task Steal<T>(PlayerChoiceContext ctx, Creature targets, CardModel card)
        where T : PowerModel
    {
        return Steal<T>(ctx, [targets], card);
    }

    public static async Task Steal<T>(PlayerChoiceContext ctx, IEnumerable<Creature> targets, CardModel card)
        where T : PowerModel
    {
        var a = card.DynamicVars.Power<T>().BaseValue;
        var player = card.Owner.Creature;
        await PowerCmd.Apply<T>(ctx, targets, -a, player, card);
        await PowerCmd.Apply<T>(ctx, player, a, player, card);
    }


    public static Creature? GainPet<T>(Player summoner) where T : MonsterModel
    {
        var combatState = summoner.Creature.CombatState;
        ArgumentNullException.ThrowIfNull(combatState);
        ArgumentNullException.ThrowIfNull(summoner.PlayerCombatState);
        return combatState.Allies.FirstOrDefault(c => c.Monster is T && c.PetOwner == summoner);
    }

    public static async Task<Creature> Summon<T>(
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