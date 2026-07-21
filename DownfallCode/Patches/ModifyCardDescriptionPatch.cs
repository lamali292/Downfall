using Downfall.DownfallCode.Interfaces;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
public static class ModifyCardDescriptionPatch
{
    private static bool Prefix(CardModel __instance, ref LocString __result)
    {
        if (__instance is not IModfyCardDescription card) return true;
        __result = card.ModifyDescription(__result);
        return false;
    }
}