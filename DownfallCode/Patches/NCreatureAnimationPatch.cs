using Downfall.DownfallCode.Interfaces;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Patches;


[HarmonyPatch(typeof(NCreature), nameof(NCreature.SetAnimationTrigger))]
public static class NCreatureAnimationPatch
{
    private static void Postfix(NCreature __instance, string trigger)
    {
        if (__instance.Visuals is IAnimatedVisuals downfallAnimation)
            downfallAnimation.OnAnimationTrigger(trigger);
    }
}

[HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
public static class NCreatureDeathAnimationPatch
{
    private static void Postfix(NCreature __instance)
    {
        if (__instance.Visuals is IAnimatedVisuals downfallAnimation)
            downfallAnimation.OnAnimationTrigger("Dead");
    }
}


