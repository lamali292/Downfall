using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Vfx;

[GlobalClass]
public partial class NSlimeCreatureVisuals : NCreatureVisuals
{
    public override void _Ready()
    {
        base._Ready();
        var premultMat = new CanvasItemMaterial
        {
            BlendMode = CanvasItemMaterial.BlendModeEnum.PremultAlpha
        };
        if (SpineBody != null)
            SpineBody.SetNormalMaterial(premultMat);
        else
            GetCurrentBody().Material = premultMat;
    }
}

public static class MySlimeDeathIsolationPatches
{
    // A private tracking set completely isolated to your own mod's lifecycle
    private static readonly HashSet<NCreature> DyingSlimes = new();

    [HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
    public static class SlimeOnlyDeathAnimStartPatch
    {
        public static void Prefix(NCreature __instance, ref bool shouldRemove)
        {
            if (__instance.Entity.Monster is not SlimeModel) return;
            DyingSlimes.Add(__instance);
            shouldRemove = true;
            NCombatRoom.Instance?.RemoveCreatureNode(__instance);
        }
    }

    [HarmonyPatch(typeof(NCreature), nameof(NCreature.GetCurrentAnimationTimeRemaining))]
    public static class SlimeOnlyDeathAnimTimePatch
    {
        public static bool Prefix(NCreature __instance, ref float __result)
        {
            if (!DyingSlimes.Contains(__instance))
                return true;
            DyingSlimes.Remove(__instance);
            __result = 0f;
            return false;
        }
    }
}