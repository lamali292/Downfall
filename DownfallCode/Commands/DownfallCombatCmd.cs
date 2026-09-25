using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Commands;

/// <summary>
///     One-off combat side-effects triggered by cards: the enemy retaliating against the player,
///     and stealing a power's stacks from a target. Split out of <see cref="DownfallCmd" />, which
///     owns generic card/creature predicates instead.
/// </summary>
public class DownfallCombatCmd
{
    public static async Task EnemyAttackPlayer(PlayerChoiceContext ctx, CardPlay cardPlay, CardModel card)
    {
        var monster = cardPlay.Target?.Monster;
        if (cardPlay.Target == null || monster == null) return;
        if (!cardPlay.Target.IsAlive) return;
        var player = card.Owner;
        var attacker = monster.Creature;
        await Cmd.Wait(0.5f);

        var enemyDamage = card.DynamicVars.EnemyDamage;
        var attack = DamageCmd.Attack(enemyDamage.BaseValue);
        attack.Attacker = attacker;
        attack._attackerAnimName = "Attack";
        attack._sourceType = AttackCommand.SourceType.Monster;
        await attack
            .Targeting(player.Creature)
            .WithValueProp(enemyDamage.Props)
            .WithHitFx("vfx/vfx_attack_slash", "event:/sfx/characters/silent/silent_attack")
            .Execute(ctx);
    }

  
}
