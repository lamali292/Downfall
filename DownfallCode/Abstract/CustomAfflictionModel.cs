using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using BaseLib.Abstracts;

namespace Downfall.DownfallCode.Abstract;

public abstract class CustomAfflictionModel : AfflictionModel, ICustomModel
{
    /// <summary>
    /// Override this or place your affliction's overlay scene at the default path
    /// used by the base game ("cards/overlays/afflictions/...").
    /// Return null to fall through to the default OverlayPath.
    /// </summary>
    protected virtual string? CustomOverlayPath => null;

    [HarmonyPatch(typeof(AfflictionModel), nameof(OverlayPath), MethodType.Getter)]
    private static class OverlayPatch
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
}