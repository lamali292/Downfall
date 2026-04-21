using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Downfall.Code.Events;

public interface IModifyCollectorDoomDamage
{
    int ModifyCollectorDoomDamage(Creature creature, int current);
}