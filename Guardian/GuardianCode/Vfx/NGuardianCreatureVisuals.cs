using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Guardian.GuardianCode.Vfx;

[GlobalClass]
public partial class NGuardianCreatureVisuals : NCreatureVisuals
{
    private const float DefaultMix = 0.2f;
    private const float ToIdleMix  = 0.35f;
    private const float AttackMix  = 0.1f;
    private const float HitMix     = 0.05f;

    private MegaSprite?         _sprite;
    private MegaAnimationState? _animState;

    public bool IsDefensive { get; set; }

    public override void _Ready()
    {
        base._Ready();

        var premultMat = new CanvasItemMaterial
        {
            BlendMode = CanvasItemMaterial.BlendModeEnum.PremultAlpha
        };

        _sprite = SpineBody;
        _sprite?.SetNormalMaterial(premultMat);

        _animState = _sprite?.GetAnimationState();

        _animState?.SetAnimation("idle");
    }

    public void OnAnimationTrigger(string trigger)
    {
        switch (trigger)
        {
            case "Idle":
                _animState?.SetAnimation(IdleAnim)
                          ?.SetMixDuration(DefaultMix);
                break;

            case "Hit":
                _animState?.SetAnimation("Hit", false)
                          ?.SetMixDuration(HitMix);
                _animState?.AddAnimation(IdleAnim)
                          .SetMixDuration(ToIdleMix);
                break;
            case "Cast":
            case "Attack":
            case "Dead":
                break;
        }
    }

    private string IdleAnim => IsDefensive ? "defensive" : "idle";
}

[HarmonyPatch(typeof(NCreature), nameof(NCreature.SetAnimationTrigger))]
public static class GuardianAnimationPatch
{
    private static void Postfix(NCreature __instance, string trigger)
    {
        if (__instance.Visuals is NGuardianCreatureVisuals guardianVisuals)
            guardianVisuals.OnAnimationTrigger(trigger);
    }
}

[HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
public static class GuardianDeathAnimPatch
{
    private static void Postfix(NCreature __instance)
    {
        if (__instance.Visuals is NGuardianCreatureVisuals guardianVisuals)
            guardianVisuals.OnAnimationTrigger("Dead");
    }
}