using Downfall.DownfallCode.Audio;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Audio;

namespace Downfall.DownfallCode.Patches;

internal static class SfxOverride
{
    public static bool ShouldRunOriginal(string path) =>
        string.IsNullOrEmpty(path) || !FmodStudio.TryPlayEvent(path);
}

// SfxCmd.Play and NAudioManager.PlayOneShot can each get inlined by the JIT, so a
// single patch site misses cases. We patch every entry point; returning false skips
// the original, so a chained call only triggers the override once.

[HarmonyPatch(typeof(OneTimeInitialization), nameof(OneTimeInitialization.ExecuteDeferred))]
internal static class DeferredInitializationFmodFlushPatch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        try
        {
            FmodStudio.OnDeferredInitializationCompleted();
        }
        catch (Exception ex)
        {
            DownfallMainFile.Logger.Warn($"[Audio] deferred FMOD flush hook failed: {ex.Message}");
        }
    }
}

[HarmonyPatch(typeof(SfxCmd), nameof(SfxCmd.Play), typeof(string), typeof(float))]
internal static class SfxPlayPatch
{
    [HarmonyPrefix]
    public static bool Prefix(string sfx) => SfxOverride.ShouldRunOriginal(sfx);
}

[HarmonyPatch(typeof(NAudioManager), nameof(NAudioManager.PlayOneShot), typeof(string), typeof(float))]
internal static class PlayOneShotPatch
{
    [HarmonyPrefix]
    public static bool Prefix(string path) => SfxOverride.ShouldRunOriginal(path);
}

[HarmonyPatch(typeof(NAudioManager), nameof(NAudioManager.PlayOneShot),
    typeof(string), typeof(Dictionary<string, float>), typeof(float))]
internal static class PlayOneShotDictPatch
{
    [HarmonyPrefix]
    public static bool Prefix(string path) => SfxOverride.ShouldRunOriginal(path);
}