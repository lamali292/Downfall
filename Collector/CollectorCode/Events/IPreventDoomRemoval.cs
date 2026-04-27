using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Collector.CollectorCode.Events;

public interface IPreventDoomRemoval
{
    bool PreventDoomRemoval(Creature creature);
}