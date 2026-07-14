using Downfall.DownfallCode.Abstract;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(NIntent), nameof(NIntent.UpdateVisuals))]
internal static class CustomIntentLabelPatch
{
    private static void Postfix(NIntent __instance)
    {
        if (__instance._intent is not CustomIntent custom)
            return;

        __instance._valueLabel.Text = custom.GetIntentLabel(__instance._targets, __instance._owner)
            .GetFormattedText();
    }
}