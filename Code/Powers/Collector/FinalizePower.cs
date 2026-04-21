using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Powers.Collector;

public class FinalizePower : CollectorPowerModel
{
    public FinalizePower() : base(PowerType.Debuff)
    {
        WithVars(new OwnerVar());
    }

    public override bool ShouldPowerBeRemovedAfterOwnerDeath()
    {
        return false;
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,
        bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature != Owner || Applier == null) return;
        await CreatureCmd.Heal(Applier, Amount);
        await PowerCmd.Remove(this);
        await Cmd.Wait(1);
    }

    private class OwnerVar() : DynamicVar("Owner", 0M)
    {
        private PowerModel? _power;

        public override void SetOwner(AbstractModel model)
        {
            base.SetOwner(model);
            _power = model as PowerModel;
        }

        public override string ToString()
        {
            return _power?.Owner.Name ?? "Unknown";
        }
    }
}