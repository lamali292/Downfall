using Downfall.DownfallCode.Extensions;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Commands;

public class DownfallCmd
{
    
    public static Task GainTempHp(PlayerChoiceContext ctx, CardModel card)
     => PowerCmd.Apply<TempHpPower>(ctx, card.Owner.Creature, card.DynamicVars["TempHP"].BaseValue, card.Owner.Creature,
         card);
    
    
    public static async Task EnemyAttackPlayer(PlayerChoiceContext ctx, CardPlay cardPlay, CardModel card)
    {
        var monster = cardPlay.Target?.Monster;
        if (cardPlay.Target == null || monster == null) return;
        if (!cardPlay.Target.IsAlive) return;
        var player = card.Owner;
        var combatState = card.CombatState;
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
}