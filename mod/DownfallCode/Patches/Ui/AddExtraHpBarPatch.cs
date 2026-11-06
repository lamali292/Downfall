using Downfall.DownfallCode.Vfx;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(NCreatureStateDisplay))]
public class AddExtraHpBarPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(NCreatureStateDisplay.SetCreature))]
    public static void SetCreaturePostfix(NCreatureStateDisplay __instance, Creature creature)
    {
        if (creature.Player == null) return;
        if (__instance.GetNodeOrNull("ExtraStatusBar") != null) return;

        var existingBar = __instance.GetNode<NHealthBar>("%HealthBar");
        var extraBar = ResourceLoader.Load<PackedScene>("res://Downfall/scenes/combat/status_bar.tscn")
            .Instantiate<NStatusBar>();

        extraBar.Name = "ExtraStatusBar";
        extraBar.Position = existingBar.Position + new Vector2(0, -existingBar.Size.Y - 25);
        extraBar.Visible = false;
        __instance.AddChild(extraBar);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NCreatureStateDisplay.SetCreatureBounds))]
    public static void SetCreatureBoundsPostfix(NCreatureStateDisplay __instance, Control bounds)
    {
        var extraBar = __instance.GetNodeOrNull<NStatusBar>("ExtraStatusBar");
        extraBar?.UpdateLayoutForCreatureBounds(bounds);
    }
}