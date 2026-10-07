using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Patches;

/// <summary>
/// The game skips <c>ModelDb.Preload()</c> when running outside the editor-player as a startup optimization.
/// BaseLib registers every scene-conversion from a postfix on that method, so it needs to run anyway when this
/// mod is loaded while playing its own project directly in the Godot editor (see editor/CLAUDE.md).
/// </summary>
/// <remarks>
/// The game makes a few deviations based on whether it runs in the Editor-Player or not. Patches like this one
/// exist to work around those. This is likely incomplete - what needs patching depends on the mods in use.
/// Registered unconditionally in <see cref="Downfall.DownfallCode.Utils.DownfallPatchManager"/> - the
/// <c>OS.HasFeature("editor")</c> check below is what actually gates it, since it's a runtime fact about the
/// current process, not something resolvable at compile time.
/// </remarks>
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
