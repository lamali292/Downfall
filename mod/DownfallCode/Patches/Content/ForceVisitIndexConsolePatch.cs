using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ancients;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(AncientDialogueSet), nameof(AncientDialogueSet.GetValidDialogues))]
internal static class ForceVisitIndexConsolePatch
{
    private static void Prefix(ref int charVisits, ref int totalVisits)
    {
        if (AncientDebug.ForcedVisitIndex is not { } v) return;
        charVisits = v;
        totalVisits = Math.Max(totalVisits, 1);
        AncientDebug.ForcedVisitIndex = null;
    }
}