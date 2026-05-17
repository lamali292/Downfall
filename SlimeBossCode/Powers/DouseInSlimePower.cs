using BaseLib.Patches.Localization;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Events;

namespace SlimeBoss.SlimeBossCode.Powers;

public class DouseInSlimePower : SlimeBossPowerModel, IAddDumbVariablesToPowerDescription, IModifyGoopConsume
{
    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        description.Add("IsApplierYou", LocalContext.IsMe(Applier));
    }

    public int ModifyGoopConsume(int amount, Creature creature, Creature? applier)
    {
        return Applier == applier && Owner == creature ? 0 : amount;
    }

    public Task AfterModifyingGoopConsume(Creature creature, Creature? applier)
    {
        return PowerCmd.Decrement(this);
    }
}