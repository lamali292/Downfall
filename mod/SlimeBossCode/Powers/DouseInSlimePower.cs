using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Events;

namespace SlimeBoss.SlimeBossCode.Powers;

public class DouseInSlimePower() : SlimeBossPowerModel(PowerType.Debuff), IModifyGoopConsume
{
    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;


    public int ModifyGoopConsume(int amount, Creature creature, Creature? applier)
    {
        return Applier == applier && Owner == creature ? 0 : amount;
    }

    // Goop consumption decrements this power itself (AfterModifyingGoopConsume); when the target has
    // no Goop from the applier there is no consumption, so the attack must use up the Douse here.
    public override Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (command.Attacker != Applier || !command.DamageProps.IsPoweredAttack() ||
            command.Results.SelectMany(r => r).All(r => r.Receiver != Owner) ||
            Owner.Powers.OfType<GoopPower>().Any(g => g.Applier == Applier))
            return Task.CompletedTask;
        return PowerCmd.Decrement(this);
    }

    public Task AfterModifyingGoopConsume(Creature creature, Creature? applier)
    {
        return PowerCmd.Decrement(this);
    }
}