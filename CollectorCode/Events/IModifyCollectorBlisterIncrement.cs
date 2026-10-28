using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Collector.CollectorCode.Events;

public interface IModifyCollectorBlisterIncrement
{
    int ModifyCollectorBlisterIncrement(Creature creature, int current);
}