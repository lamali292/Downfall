using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Extensions;

public static class CreatureExtensions
{
    public static bool IsAfflicted(this Creature creature)
    {
        return creature.HasPower<VulnerablePower>() && creature.HasPower<WeakPower>();
    }
}