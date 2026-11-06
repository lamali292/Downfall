using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;

namespace Downfall.DownfallCode.Patches;

// Vanilla's own SfxCmd.Play / NAudioManager.PlayOneShot resolve Downfall's event paths
// natively once our banks are loaded (verified in-game: they share the same FmodServer
// Studio System instance as the base game). No interception needed there — this patch
// only exists to flush our queued RegisterBank calls once deferred init completes.
[HarmonyPatch(typeof(OneTimeInitialization), nameof(OneTimeInitialization.ExecuteDeferred))]
internal static class DeferredInitializationFmodFlushPatch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        try
        {
            Audio.FmodStudio.OnDeferredInitializationCompleted();
        }
        catch (Exception ex)
        {
            DownfallMainFile.Logger.Warn($"[Audio] deferred FMOD flush hook failed: {ex.Message}");
        }
    }
}
