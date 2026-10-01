using Collector.CollectorCode.Vfx;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Collector.CollectorCode.Patches;

/// <summary>Adds the Reserve counter next to the energy counter for the local player and animates it with the combat UI.</summary>
[HarmonyPatch(typeof(NCombatUi))]
internal static class ReserveCounterPatch
{
    [HarmonyPatch(nameof(NCombatUi.Activate))]
    [HarmonyPostfix]
    private static void AddCounter(NCombatUi __instance, CombatState state)
    {
        var player = LocalContext.GetMe(state);
        if (player == null) return;

        var counter = NCollectorEnergyCounter.Create(player);
        counter.Position = new Vector2(80f, 80f);
        counter.Scale = new Vector2(0.6f, 0.6f);
        __instance.EnergyCounterContainer.AddChild(counter);
        counter.AnimIn();
    }

    [HarmonyPatch(nameof(NCombatUi.AnimOut))]
    [HarmonyPostfix]
    private static void AnimCountersOut(NCombatUi __instance)
    {
        foreach (var child in __instance.EnergyCounterContainer.GetChildren())
            if (child is NCollectorEnergyCounter counter)
                counter.AnimOut();
    }
}
