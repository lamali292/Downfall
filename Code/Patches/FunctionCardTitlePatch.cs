using Downfall.Code.Cards.Automaton.Token;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Patches;

[HarmonyPatch(typeof(CardModel), "get_Title")]
public static class FunctionCardTitlePatch
{
    private static bool Prefix(CardModel __instance, ref string __result)
    {
        if (__instance is not FunctionCard fc) return true; // run original for all other cards

        __result = fc.GetDynamicTitle();
        return false; // skip original
    }
}