using MegaCrit.Sts2.Core.Entities.Creatures;

namespace SlimeBoss.SlimeBossCode.Events;

public interface IPreventConsumeWeakRemoval
{
    /// <summary>
    /// Asked once per Consume on a living target, before its Weak stack is removed. Return true to leave the
    /// Weak untouched; the listener is then expected to account for it (e.g. spend a use). The first listener
    /// returning true wins, so only one of them is spent per Consume.
    /// </summary>
    bool TryPreventWeakRemoval(Creature target, Creature consumer);
}
