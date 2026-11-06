using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(OneTimeInitialization), nameof(OneTimeInitialization.ExecuteDeferred))]
internal static class IsEditorPatches
{
    [HarmonyPostfix]
    private static void ForcePreloadInEditor()
    {
        if (OS.HasFeature("editor")) // Same check the game uses
            ModelDb.Preload();
    }
}
