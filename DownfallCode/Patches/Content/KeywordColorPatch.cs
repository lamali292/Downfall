using Downfall.DownfallCode.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(CardKeywordExtensions), nameof(CardKeywordExtensions.GetCardText))]
internal static class KeywordColorPatch
{
    [HarmonyPostfix]
    private static void Postfix(CardKeyword keyword, ref string __result)
    {
        if (!KeywordColorRegistry.TryGetColor(keyword, out var color)) return;

        __result = __result.Replace("[gold]", $"[{color}]")
            .Replace("[/gold]", $"[/{color}]");
    }
}