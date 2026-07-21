using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Patches;

[HarmonyPatch(typeof(PersonalHivePower), nameof(PersonalHivePower.AfterDamageReceived))]
internal static class PersonalHivePowerSlimePatch
{
    private static bool Prefix(Creature? dealer, ref Task __result)
    {
        if (dealer?.Monster is not SlimeModel) return true;
        __result = Task.CompletedTask;
        return false;
    }
}