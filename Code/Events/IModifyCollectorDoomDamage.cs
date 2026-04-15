using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.Code.Events;

public interface IModifyCollectorDoomDamage
{
    int ModifyCollectorDoomDamage(Creature creature, int current);
}