using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Utils;

public static class ForceUpgradeHelper
{
    private static readonly ConditionalWeakTable<CardModel, StrongBox<int>> ForceUpgraded = new();

    public static void ForceUpgrade(CardModel card, int times = 1)
    {
        var box = ForceUpgraded.GetOrCreateValue(card);
        for (var i = 0; i < times; i++)
        {
            box.Value = card._currentUpgradeLevel + 1;
            card.UpgradeInternal();
            card.FinalizeUpgradeInternal();
            box.Value = card._currentUpgradeLevel;
        }
    }


    [HarmonyPatch(typeof(CardModel), "get_MaxUpgradeLevel")]
    public static class MaxUpgradeLevelPatch
    {
        public static void Postfix(CardModel __instance, ref int __result)
        {
            if (ForceUpgraded.TryGetValue(__instance, out var box))
                __result = Math.Max(__result, box.Value);
        }
    }
}