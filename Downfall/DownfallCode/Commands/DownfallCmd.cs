using BaseLib.Extensions;
using Downfall.DownfallCode.Extensions;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Commands;

public class DownfallCmd
{

    public static Task GainTempHp(PlayerChoiceContext ctx, CardModel card)
        => GainTempHp( ctx, card, card.DynamicVars["TempHP"].BaseValue);
    
    public static Task GainTempHp(PlayerChoiceContext ctx, CardModel card, decimal tempHp)
        => PowerCmd.Apply<TempHpPower>(ctx, card.Owner.Creature, tempHp, card.Owner.Creature,
            card);
    
    public static Task GainTempHp(PlayerChoiceContext ctx, Creature creature, decimal tempHp)
        => PowerCmd.Apply<TempHpPower>(ctx, creature, tempHp, creature,
            null);


    public static int GetTempHpAmount(Creature creature)
        => creature.GetPowerAmount<TempHpPower>();

    
    
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
        switch  (card.TargetType)
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
                await Steal<T>(ctx, card.CombatState.HittableEnemies.TakeRandom(1, card.CombatState.RunState.Rng.CombatTargets), card);
                break;
        }
    }


    public static Task Steal<T>(PlayerChoiceContext ctx, Creature targets, CardModel card)
        where T : PowerModel
        => Steal<T>(ctx, [targets], card);
    
    public static async Task Steal<T>(PlayerChoiceContext ctx, IEnumerable<Creature> targets, CardModel card)
    where T : PowerModel
    {
        var a = card.DynamicVars.Power<T>().BaseValue;
        var player =  card.Owner.Creature;
        await PowerCmd.Apply<T>(ctx, targets, -a, player, card);
        await PowerCmd.Apply<T>(ctx, player, a, player, card);
    }
}