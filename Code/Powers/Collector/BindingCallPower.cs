using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;

namespace Downfall.Code.Powers.Collector;

public class BindingCallPower : CollectorPowerModel
{
    public override bool ShouldPowerBeRemovedAfterOwnerDeath()
    {
        return false;
    }

    public override async Task AfterAttack(AttackCommand command)
    {
        if (command.Attacker == null || Owner.PetOwner == null || !Owner.IsAlive) return;
        if (Owner.PetOwner == command.Attacker.Player)
        {
            var target = Owner.PetOwner.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
            if (target != null)
                await PowerCmd.Apply<CollectorDoomPower>(target, Amount, Owner, null);
        }
    }
}