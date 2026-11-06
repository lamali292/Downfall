using MegaCrit.Sts2.Core.Entities.Creatures;

namespace SlimeBoss.SlimeBossCode.Events;

public interface IModifyGoopConsume
{
    int ModifyGoopConsume(int amount, Creature creature, Creature? applier);
    Task AfterModifyingGoopConsume(Creature creature, Creature? applier);
}