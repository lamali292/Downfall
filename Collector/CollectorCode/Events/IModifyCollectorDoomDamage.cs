using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Collector.CollectorCode.Events;

public interface IModifyCollectorDoomDamage
{
    int ModifyCollectorDoomDamage(Creature creature, int current);
}