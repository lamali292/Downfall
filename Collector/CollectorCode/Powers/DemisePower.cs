using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Collector.CollectorCode.Powers;

public class DemisePower() : CollectorPowerModel(PowerType.Debuff), IPreventDoomRemoval
{
    public bool PreventDoomRemoval(Creature creature)
    {
        if (creature != Owner || Amount <= 0) return false;
        PowerCmd.Decrement(this);
        return true;
    }
}