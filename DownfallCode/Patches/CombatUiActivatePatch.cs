using Downfall.DownfallCode.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
internal static class CombatUiActivatePatch
{
    [HarmonyPostfix]
    private static void Postfix(CombatState state)
        => CombatUiHooks.RaiseActivate(state);
}