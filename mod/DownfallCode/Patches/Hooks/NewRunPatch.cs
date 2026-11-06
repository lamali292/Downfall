using Downfall.DownfallCode.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Runs;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(RunState), nameof(RunState.CreateForNewRun))]
internal static class NewRunPatch
{
    [HarmonyPostfix]
    private static void Postfix(RunState __result)
    {
        RunHooks.RaiseNewRun(__result);
    }
}