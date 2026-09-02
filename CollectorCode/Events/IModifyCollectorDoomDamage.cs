using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Collector.CollectorCode.Events;

public interface IModifyCollectorMiasmaIncrement
{
    int ModifyCollectorMiasmaIncrement(Creature creature, int current);
}