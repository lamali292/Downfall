using Downfall.DownfallCode.Interfaces;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Patches;

/// <summary>
/// <see cref="CardModel.DowngradeInternal" /> (used by e.g. the Reflections event) always resets
/// <c>CurrentUpgradeLevel</c> to 0. That's correct for an ordinary +1 card, but for an
/// <see cref="IStackingUpgradeCard" /> (whose MaxUpgradeLevel grows with its own upgrade level, so
/// it can sit at +2, +3, ...) it wipes every level at once instead of removing just one. Re-apply
/// all but one of the removed levels so downgrade only ever removes a single level here too.
/// </summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.DowngradeInternal))]
public static class StackingUpgradeDowngradePatch
{
    public static void Prefix(CardModel __instance, out int __state)
    {
        __state = __instance.CurrentUpgradeLevel;
    }

    public static void Postfix(CardModel __instance, int __state)
    {
        if (__instance is not IStackingUpgradeCard) return;
        for (var i = 0; i < __state - 1; i++)
        {
            __instance.UpgradeInternal();
            __instance.FinalizeUpgradeInternal();
        }
    }
}
