using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Powers;

public class FinalizePower() : CollectorPowerModel(PowerType.Debuff)
{
    public override bool ShouldPowerBeRemovedAfterOwnerDeath()
    {
        return false;
    }

    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,
        bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature != Owner || Applier == null) return;
        await CreatureCmd.Heal(Applier, Amount);
        await PowerCmd.Remove(this);
        await Cmd.Wait(1);
    }
}