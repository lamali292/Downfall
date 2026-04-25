using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.Code.Vfx.Hexaghost;

[GlobalClass]
public partial class NHexaghostCreatureVisuals : NCreatureVisuals
{
    public NHexaghostVisuals? Visuals;

    public override void _Ready()
    {
        base._Ready();
        Visuals = GetNode<NHexaghostVisuals>("%Hexaghost");
    }


    public void OnAnimationTrigger(string trigger)
    {
        Visuals?.OnAnimationTrigger(trigger);
    }
}

[HarmonyPatch(typeof(NCreature), nameof(NCreature.SetAnimationTrigger))]
public static class HexaghostAnimationPatch
{
    private static void Postfix(NCreature __instance, string trigger)
    {
        if (__instance.Visuals is NHexaghostCreatureVisuals hexVisuals)
            hexVisuals.OnAnimationTrigger(trigger);
    }
}

[HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
public static class HexaghostDeathAnimPatch
{
    private static void Postfix(NCreature __instance)
    {
        if (__instance.Visuals is NHexaghostCreatureVisuals hexVisuals)
            hexVisuals.OnAnimationTrigger("Dead");
    }
}