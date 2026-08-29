using Downfall.DownfallCode.Interfaces;
using HarmonyLib;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch]
public static class NCreatureAnimationPatch
{
    [HarmonyPatch(typeof(NCreature), nameof(NCreature.SetAnimationTrigger))]
    [HarmonyPostfix]
    private static void OnTrigger(NCreature __instance, string trigger)
    {
        if (__instance.Visuals is IAnimatedVisuals downfallAnimation)
            downfallAnimation.OnAnimationTrigger(trigger);
    }

    [HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
    [HarmonyPostfix]
    private static void OnDeath(NCreature __instance)
    {
        if (__instance.Visuals is IAnimatedVisuals downfallAnimation)
            downfallAnimation.OnAnimationTrigger("Dead");

        if (__instance.Entity.Player is { } player)
            HexaghostVisualsBridge.FadeFlamesOnDeath(player);
    }

    [HarmonyPatch(typeof(NCreature), nameof(NCreature.StartReviveAnim))]
    [HarmonyPostfix]
    private static void OnRevive(NCreature __instance)
    {
        if (__instance.Visuals is IAnimatedVisuals downfallAnimation)
            downfallAnimation.OnAnimationTrigger("Revive");

        if (__instance.Entity.Player is { } player)
            HexaghostVisualsBridge.FadeFlamesOnRevive(player);
    }
}