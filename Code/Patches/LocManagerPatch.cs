using Downfall.Code.Localization;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using SmartFormat;

namespace Downfall.Code.Patches;

[HarmonyPatch(typeof(LocManager), nameof(LocManager.Initialize))]
public static class LocManagerPatch
{
    [HarmonyPostfix]
    private static void AddCustomFormatters()
    {
        Smart.Default.AddExtensions(new PowerIconFormatter(), new FinisherFormatter());
    }
}