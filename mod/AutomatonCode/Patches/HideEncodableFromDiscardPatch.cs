using Automaton.AutomatonCode.Core;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Patches;

/// <summary>
///     An encodable card that would go to the discard pile after being played goes nowhere instead
///     (it is encoded, not discarded). Applied to the location <c>CardModel.GetResultLocationForCardPlay</c>
///     reports, i.e. before any <c>ModifyCardPlayResultLocation</c> listener (Rebound, ...) sees it.
/// </summary>
[HarmonyPatch(typeof(CardModel), "GetResultLocationForCardPlay")]
internal static class HideEncodableFromDiscardPatch
{
    [HarmonyPostfix]
    private static void Postfix(CardModel __instance, ref CardLocation __result)
    {
        if (__result.pileType != PileType.Discard || !AutomatonCmd.IsEncodable(__instance)) return;
        __result = __result with { pileType = PileType.None };
    }
}
