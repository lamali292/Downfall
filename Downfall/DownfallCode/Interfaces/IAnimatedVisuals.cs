using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Interfaces;

public interface IAnimatedVisuals
{
    void OnAnimationTrigger(string trigger);
}


[HarmonyPatch(typeof(NCreature), nameof(NCreature.SetAnimationTrigger))]
public static class HexaghostAnimationPatch
{
    private static void Postfix(NCreature __instance, string trigger)
    {
        if (__instance.Visuals is IAnimatedVisuals hexVisuals)
            hexVisuals.OnAnimationTrigger(trigger);
    }
}

[HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
public static class HexaghostDeathAnimPatch
{
    private static void Postfix(NCreature __instance)
    {
        if (__instance.Visuals is IAnimatedVisuals hexVisuals)
            hexVisuals.OnAnimationTrigger("Dead");
    }
}