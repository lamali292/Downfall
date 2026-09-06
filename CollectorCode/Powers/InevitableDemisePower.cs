using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Collector.CollectorCode.Powers;

public class InevitableDemisePower() : CollectorPowerModel(PowerType.Debuff), IModifyCollectorMiasmaIncrement
{
    public int ModifyCollectorMiasmaIncrement(Creature creature, int current)
    {
        return creature == Owner ? current + Amount : current;
    }
}