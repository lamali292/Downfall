using Downfall.Code.Character;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Saves.Managers;

namespace Downfall.Code.Patches;

[HarmonyPatch]
internal class ProgressSaveManager_Patches
{
    [HarmonyPatch(typeof(ProgressSaveManager))]
    [HarmonyPatch("ObtainCharUnlockEpoch")]
    [HarmonyPatch([typeof(Player), typeof(int)])]
    private static class ObtainEpochPatch
    {
        private static bool Prefix(ProgressSaveManager __instance, Player localPlayer, int act)
        {
            // Skip method for Downfall or handle custom logic
            // TODO: general check or check if base lib fixed this
            return localPlayer.Character is not Automaton;
        }
    }
}