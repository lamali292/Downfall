using Downfall.DownfallCode.Abstract;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(AfflictionModel), nameof(AfflictionModel.OverlayPath), MethodType.Getter)]
internal static class AfflictionModelOverlayPathPatch
{
    private static bool Prefix(AfflictionModel __instance, ref string __result)
    {
        if (__instance is not CustomAfflictionModel customAfflictionModel)
            return true;

        var custom = customAfflictionModel.CustomOverlayPath;
        if (custom == null)
            return true;

        __result = custom;
        return false;
    }
}