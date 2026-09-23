using System.Reflection;
using Downfall.DownfallCode.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Patches;

// Records which registered ICustomUnplayableReason (if any) is why this card is BlockedByCardLogic, by
// OR-ing its flag into `reason` and stashing the card itself in the unused `preventer` out-param.
[HarmonyPatch]
internal static class CardCanPlayCustomReasonPatch
{
    private static MethodBase TargetMethod() =>
        AccessTools.Method(typeof(CardModel), nameof(CardModel.CanPlay),
            [typeof(UnplayableReason).MakeByRefType(), typeof(AbstractModel).MakeByRefType()]);

    [HarmonyPostfix]
    private static void Postfix(CardModel __instance, ref UnplayableReason reason, ref AbstractModel? preventer)
    {
        if (!reason.HasFlag(UnplayableReason.BlockedByCardLogic)) return;

        foreach (var custom in CustomUnplayableReasonRegistry.All)
        {
            if (!custom.AppliesTo(__instance)) continue;
            reason |= custom.Flag;
            preventer ??= __instance;
        }
    }
}

// Substitutes a registered custom reason's dialogue line for the generic "UNPLAYABLE" text, but only when
// none of the higher-priority reasons the vanilla method checks first (energy, stars, living allies, a
// blocking hook) are also set - matching vanilla's own priority order.
[HarmonyPatch(typeof(UnplayableReasonExtensions), nameof(UnplayableReasonExtensions.GetPlayerDialogueLine))]
internal static class UnplayableReasonDialogueLinePatch
{
    private const UnplayableReason HigherPriorityReasons =
        UnplayableReason.NoLivingAllies | UnplayableReason.EnergyCostTooHigh |
        UnplayableReason.StarCostTooHigh | UnplayableReason.BlockedByHook;

    [HarmonyPostfix]
    private static void Postfix(UnplayableReason reason, AbstractModel? preventer, ref LocString? __result)
    {
        if ((reason & HigherPriorityReasons) != 0) return;
        if (preventer is not CardModel card) return;

        foreach (var custom in CustomUnplayableReasonRegistry.All)
        {
            if (!reason.HasFlag(custom.Flag)) continue;
            __result = custom.GetDialogueLine(card);
            return;
        }
    }
}
