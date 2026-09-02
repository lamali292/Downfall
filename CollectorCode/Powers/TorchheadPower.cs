using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class TorchheadPower() : CollectorPowerModel(PowerType.Buff, PowerStackType.Single)
{
    public override bool ShouldPlayVfx => false;

    public override Creature ModifyUnblockedDamageTarget(
        Creature target,
        decimal _,
        ValueProp props,
        Creature? __)
    {
        return target != Owner.PetOwner?.Creature || Owner.IsDead || !props.IsPoweredAttack() ? target : Owner;
    }

    /// <summary>This is so Osty won't receive powers while it is dead</summary>
    public override bool ShouldAllowHitting(Creature creature) => creature.IsAlive;

    public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)
    {
        return creature != Owner;
    }

    public override bool ShouldPowerBeRemovedAfterOwnerDeath() => false;
}