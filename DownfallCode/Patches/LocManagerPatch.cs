using Downfall.DownfallCode.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(LocManager), nameof(LocManager.LoadLocFormatters))]
public static class LocManagerPatch
{
    [HarmonyPostfix]
    private static void Postfix()
    {
        LocFormatterRegistry.ApplyAll();
    }
}