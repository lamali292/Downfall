using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Powers;

public class BindingCallPower : CollectorPowerModel
{
    public override bool ShouldPowerBeRemovedAfterOwnerDeath()
    {
        return false;
    }

    public override async Task AfterAttack(PlayerChoiceContext ctx, AttackCommand command)
    {
        if (command.Attacker == null || Owner.PetOwner == null || !Owner.IsAlive) return;
        if (Owner.PetOwner == command.Attacker.Player)
        {
            var target = Owner.PetOwner.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
            if (target != null)
                await PowerCmd.Apply<CollectorDoomPower>(ctx, target, Amount, Owner, null);
        }
    }
}