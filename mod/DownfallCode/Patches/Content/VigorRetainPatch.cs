using Downfall.DownfallCode.Interfaces;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(VigorPower), nameof(VigorPower.AfterAttack))]
internal static class VigorRetainPatch
{
    [HarmonyPrefix]
    private static bool Prefix(VigorPower __instance, AttackCommand command, ref Task __result)
    {
        if (command.ModelSource is not IRetainVigorCard) return true;

        __instance.GetInternalData<VigorPower.Data>().commandToModify = null;
        __result = Task.CompletedTask;
        return false;
    }
}