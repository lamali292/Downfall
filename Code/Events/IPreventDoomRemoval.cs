using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Downfall.Code.Events;

public interface IPreventDoomRemoval
{
    bool PreventDoomRemoval(Creature creature);
}