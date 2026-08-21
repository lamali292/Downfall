using HarmonyLib;
using Hermit.HermitCode.Relics;
using MegaCrit.Sts2.Core.Nodes.Potions;

namespace Hermit.HermitCode.Patches;


[HarmonyPatch(typeof(NPotionPopup), nameof(NPotionPopup.RefreshButtons))]
[Obsolete]
internal static class ShotglassLimitPatch
{
    private static void Postfix(NPotionPopup __instance)
    {
        var potion = __instance.Potion;
        var shotglass = potion?.Owner.GetRelic<Shotglass>();
        if (shotglass is { IsInCombat: true, AvailableUses: <= 0 })
            __instance._useButton.Disable();
    }
}