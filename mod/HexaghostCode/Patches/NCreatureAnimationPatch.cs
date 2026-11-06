using HarmonyLib;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Hexaghost.HexaghostCode.Patches;

[HarmonyPatch]
public static class NCreatureAnimationPatch
{
    [HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
    [HarmonyPostfix]
    private static void OnDeath(NCreature __instance)
    {
        if (__instance.Entity.Player is { } player)
            HexaghostVisualsBridge.FadeFlamesOnDeath(player);
    }

    [HarmonyPatch(typeof(NCreature), nameof(NCreature.StartReviveAnim))]
    [HarmonyPostfix]
    private static void OnRevive(NCreature __instance)
    {
        if (__instance.Entity.Player is { } player)
            HexaghostVisualsBridge.FadeFlamesOnRevive(player);
    }
}
