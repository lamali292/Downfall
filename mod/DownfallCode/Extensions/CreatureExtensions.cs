using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Extensions;

public static class CreatureExtensions
{
    extension(Creature creature)
    {
        public int GetInstancedPowerAmountSum<T>() where T : PowerModel
        {
            var power = creature.GetPowerInstances<T>();
            return power.Sum(e => e.Amount);
        }
    }
}