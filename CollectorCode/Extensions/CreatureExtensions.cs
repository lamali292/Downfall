using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Extensions;

public static class CreatureExtensions
{
    extension(Creature creature)
    {
        public bool IsAfflicted => creature.HasPower<VulnerablePower>() && creature.HasPower<WeakPower>();
    }
}