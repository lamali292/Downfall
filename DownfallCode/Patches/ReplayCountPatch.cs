using Downfall.DownfallCode.Interfaces;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.GetEnchantedReplayCount))]
internal static class ReplayCountPatch
{
    [HarmonyPostfix]
    private static void Postfix(CardModel __instance, ref int __result)
    {
        if (__instance is IModifyReplayCount card)
            __result = card.ModifyReplayCount(__result);
    }
}