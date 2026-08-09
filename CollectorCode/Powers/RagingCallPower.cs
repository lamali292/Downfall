using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class RagingCallPower : CollectorPowerModel
{
    public override bool ShouldPowerBeRemovedAfterOwnerDeath()
    {
        return false;
    }

    public override async Task AfterAttack(PlayerChoiceContext ctx, AttackCommand command)
    {
        if (command.Attacker == null || Owner.PetOwner == null || !Owner.IsAlive) return;
        if (Owner.PetOwner == command.Attacker.Player)
            // TODO: torchhead shoudld deal the damage.
            await DownfallCreatureCmd.Damage(ctx, CombatState.HittableEnemies, Amount,
                DamageProps.nonCardUnpowered, Owner, null, null);
    }
}