using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using SmartFormat.Utilities;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(LocManager), nameof(LocManager.LoadLocFormatters))]
public static class PluralRulesPatch
{
    [HarmonyPostfix]
    private static void FixChinesePlural()
    {
        var prop = typeof(PluralRules).GetProperty("IsoLangToDelegate",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (prop?.GetValue(null) is { } dict)
        {
            var indexer = dict.GetType().GetProperty("Item");
            indexer?.SetValue(dict, PluralRules.GetPluralRule("en"), ["zh"]);
        }
    }
}
