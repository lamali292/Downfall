using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Patches;

[HarmonyPatch(typeof(NCreature))]
public static class SlimeDeathPatches
{
    // A private tracking set completely isolated to your own mod's lifecycle
    private static readonly HashSet<NCreature> DyingSlimes = new();

    [HarmonyPrefix]
    [HarmonyPatch(nameof(NCreature.StartDeathAnim))]
    public static void Prefix(NCreature __instance, ref bool shouldRemove)
    {
        if (__instance.Entity.Monster is not SlimeModel) return;
        DyingSlimes.Add(__instance);
        shouldRemove = true;
        NCombatRoom.Instance?.RemoveCreatureNode(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(NCreature.GetCurrentAnimationTimeRemaining))]
    public static bool Prefix(NCreature __instance, ref float __result)
    {
        if (!DyingSlimes.Contains(__instance))
            return true;
        DyingSlimes.Remove(__instance);
        __result = 0f;
        return false;
    }
}