using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Collector.CollectorCode.Powers;

/// <summary>
///     Every time the owner takes damage from an attack hit it also loses HP equal to the Blister amount (ignoring
///     Block, like Poison), then Blister drops by 1. The amount is the damage directly; it triggers once per hit, so a
///     multi-hit attack pops several stacks.
/// </summary>
public class BlisterPower() : CollectorPowerModel(PowerType.Debuff)
{
    public override async Task AfterDamageReceived(PlayerChoiceContext ctx, Creature target, DamageResult result,
        ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // Our own HP loss below is Unpowered, so it can't retrigger this.
        if (target != Owner || !props.IsPoweredAttack() || Amount <= 0) return;

        await CreatureCmd.Damage(ctx, Owner, Amount, DamageProps.nonCardHpLoss, null, null);
        Flash();
        if (Owner.IsAlive) await PowerCmd.Decrement(this);
    }
}
