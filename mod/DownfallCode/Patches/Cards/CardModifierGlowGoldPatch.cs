using BaseLib.Abstracts;
using Downfall.DownfallCode.Abstract;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(NHandCardHolder), "get_ShouldGlowGold")]
internal static class CardModifierGlowGoldPatch
{
    private static void Postfix(NHandCardHolder __instance, ref bool __result)
    {
        if (__result) return;

        var model = __instance.CardNode?.Model;
        if (model == null) return;

        if (CardModifier.Modifiers(model).OfType<DownfallCardModifier>().Any(e => e.ShouldGlowGold))
            __result = true;
    }
}