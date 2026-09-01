using Downfall.DownfallCode.Abstract;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(PowerCmd), nameof(PowerCmd.FindExistingInstanceForStacking))]
public static class FindExistingInstanceForStackingPatch
{
    public static bool Prefix(
        PowerModel basePower,
        Creature target,
        Creature? applier,
        ref PowerModel? __result)
    {
        if (!CustomPowerInstanceType.PowerInstanceTypes.TryGetValue(basePower.InstanceType, out var isPowerSame))
            return true;
        __result = target.GetPowerInstances(basePower.Id)
            .FirstOrDefault(p => isPowerSame.Invoke(basePower, target, applier, p));
        return false;
    }
}