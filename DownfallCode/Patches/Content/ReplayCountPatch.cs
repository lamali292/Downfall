using Downfall.DownfallCode.Interfaces;
using BaseLib.Utils;
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
        __result = HookUtils.Modify<IModifyCardReplayCount, int>(__instance.CombatState, __result,
            (e, count) => e.ModifyReplayCount(__instance, count), out _);
    }
}