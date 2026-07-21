using Downfall.DownfallCode.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(CreatureCmd), nameof(CreatureCmd.KillWithoutCheckingWinCondition))]
internal static class DeathInterceptPatch
{
    [HarmonyPrefix]
    private static bool Prefix(Creature creature, bool force, ref Task __result)
    {
        if (force) return true; // forced kills are never interceptable

        var task = DeathHooks.TryIntercept(creature);
        if (task == null) return true;

        __result = task;
        return false;
    }
}