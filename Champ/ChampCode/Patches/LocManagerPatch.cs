using Champ.ChampCode.Localization;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using SmartFormat;

namespace Champ.ChampCode.Patches;

[HarmonyPatch(typeof(LocManager), nameof(LocManager.LoadLocFormatters))]
public static class LocManagerPatch
{
    [HarmonyPostfix]
    private static void AddCustomFormatters()
    {
        Smart.Default.AddExtensions(new FinisherFormatter());
    }
}