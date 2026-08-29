using Downfall.DownfallCode.Interfaces;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(EnchantmentModel), nameof(EnchantmentModel.CanEnchant))]
public static class EnchantmentModelCanEnchantCardVeto
{
    [HarmonyPostfix]
    public static void Postfix(EnchantmentModel __instance, CardModel card, ref bool __result)
    {
        if (!__result) return;
        if (card is IEnchantRestrictedCard restricted &&
            !restricted.CanBeEnchantedWith(__instance))
            __result = false;
    }
}