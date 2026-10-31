using Collector.CollectorCode.Vfx;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Collector.CollectorCode.Patches;

/// <summary>Gives every enemy a Torchhead target indicator that shows itself while Torchhead would attack it.</summary>
[HarmonyPatch(typeof(NCreature), "_Ready")]
public static class NCreatureTargetIndicatorPatch
{
    [HarmonyPostfix]
    static void Postfix(NCreature __instance)
    {
        if (__instance.Entity.Side != CombatSide.Enemy) return;

        // Anchor to the health bar's container: the base game sizes it per creature, so its rect matches each
        // enemy's visible bar, unlike the fixed-width hitbox.
        var stateDisplay = Traverse.Create(__instance).Field("_stateDisplay").GetValue<Control>();
        var healthBar = stateDisplay == null ? null : Traverse.Create(stateDisplay).Field("_healthBar").GetValue<Control>();
        var anchor = healthBar == null ? null : GetMember(healthBar, "HpBarContainer");
        anchor ??= stateDisplay == null ? null : GetMember(stateDisplay, "_hpBarHitbox");
        anchor ??= healthBar ?? __instance.Hitbox;

        // A sibling of the health bar, so it fades with the bar and the pause dim covers it too.
        var indicator = NTorchheadTargetIndicator.Create(__instance, anchor);
        indicator.Name = "TorchheadTargetIndicator";
        (stateDisplay ?? (Control)__instance).AddChild(indicator);
    }

    private static Control? GetMember(object owner, string name)
    {
        var traverse = Traverse.Create(owner);
        return traverse.Property(name).GetValue<Control>() ?? traverse.Field(name).GetValue<Control>();
    }
}
