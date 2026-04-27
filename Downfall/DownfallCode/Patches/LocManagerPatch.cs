using Downfall.DownfallCode.Localization;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using SmartFormat;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(LocManager), nameof(LocManager.LoadLocFormatters))]
public static class LocManagerPatch
{
    [HarmonyPostfix]
    private static void AddCustomFormatters()
    {
        Smart.Default.AddExtensions(new PowerIconFormatter());
    }
}