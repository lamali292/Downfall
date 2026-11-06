using Downfall.DownfallCode.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(CardModel), "get_MaxUpgradeLevel")]
public static class MaxUpgradeLevelPatch
{
    public static void Postfix(CardModel __instance, ref int __result)
    {
        if (ForceUpgradeHelper.ForceUpgraded.TryGetValue(__instance, out var box))
            __result = Math.Max(__result, box.Value);
    }
}