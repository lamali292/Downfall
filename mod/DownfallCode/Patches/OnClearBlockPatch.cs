using Downfall.DownfallCode.History;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(Creature), "ClearBlock")]
internal static class OnClearBlockPatch
{
    [HarmonyPrefix]
    private static bool SaveUnusedToHistory(Creature __instance)
    {
        var combatState = __instance.CombatState;
        if (combatState == null) return true;
        var entry = new UnusedBlockEntry(__instance.Block, __instance, combatState.RoundNumber, __instance.Side,
            CombatManager.Instance.History, combatState.Players);
        CombatManager.Instance.History.Add(combatState, entry);
        return true;
    }
}