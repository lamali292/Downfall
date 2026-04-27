using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Combat;
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
            await CreatureCmd.Damage(ctx, CombatState.HittableEnemies, Amount,
                ValueProp.Unblockable | ValueProp.Unpowered, Owner, null);
    }
}