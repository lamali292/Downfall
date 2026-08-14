using BaseLib.Audio;
using Downfall.DownfallCode.Audio;
using Downfall.DownfallCode.Utils.Sound;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Nodes.Audio;

namespace Downfall.DownfallCode.Patches;

public static class SfxOverrideRegistry
{
    public static bool TryHandleResPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return false;
        if (!FmodStudioGuidPathTable.TryGetStudioGuidForEventPath(path, out var guid)
            || !FmodStudioGuidInterop.TryNormalizeForAddon(guid, out var normalized)) return false;
        FmodStudioGateway.TryCall(FmodStudioMethodNames.PlayOneShotUsingGuid, normalized, 1f);
        return true;
    }
}

// catch the override at multiple places because some of the original functions get inlined by jit so some patches not work all the time
[HarmonyPatch(typeof(SfxCmd), nameof(SfxCmd.Play), typeof(string), typeof(float))]
internal static class SfxOverridePatch
{
    [HarmonyPrefix]
    public static bool Prefix(string sfx)
    {
        return !SfxOverrideRegistry.TryHandleResPath(sfx);
    }
}

[HarmonyPatch(typeof(NAudioManager), nameof(NAudioManager.PlayOneShot), typeof(string), typeof(float))]
internal static class PlayOneShotPatch
{
    [HarmonyPrefix]
    public static bool Prefix(string path, float volume)
    {
        return !SfxOverrideRegistry.TryHandleResPath(path);
    }
}

[HarmonyPatch(typeof(NAudioManager), nameof(NAudioManager.PlayOneShot), typeof(string),
    typeof(Dictionary<string, float>), typeof(float))]
internal static class PlayOneShotDictPatch
{
    [HarmonyPrefix]
    public static bool Prefix(string path, float volume)
    {
        return !SfxOverrideRegistry.TryHandleResPath(path);
    }
}