using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Hermit.HermitCode.Vfx;

[GlobalClass]
public partial class NHermitCreatureVisuals : NCreatureVisuals
{
	private const float DefaultMix = 0.2f;
	private const float ToIdleMix = 0.35f;
	private const float AttackMix = 0.1f;
	private const float HitMix = 0.05f;
	private MegaAnimationState? _animState;

	private MegaSprite? _sprite;

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

		_animState?.SetAnimation("Idle");
	}

	public void OnAnimationTrigger(string trigger)
	{
		switch (trigger)
		{
			case "Idle":
				_animState?.SetAnimation("Idle")
					?.SetMixDuration(DefaultMix);
				break;
			case "Hit":
				_animState?.SetAnimation("Hit", false)
					?.SetMixDuration(HitMix);
				_animState?.AddAnimation("Idle")
					.SetMixDuration(ToIdleMix);
				break;
			case "Cast":
			case "Attack":
			case "Dead":
				break;
		}
	}
}

[HarmonyPatch(typeof(NCreature), nameof(NCreature.SetAnimationTrigger))]
public static class GuardianAnimationPatch
{
	private static void Postfix(NCreature __instance, string trigger)
	{
		if (__instance.Visuals is NHermitCreatureVisuals guardianVisuals)
			guardianVisuals.OnAnimationTrigger(trigger);
	}
}

[HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
public static class GuardianDeathAnimPatch
{
	private static void Postfix(NCreature __instance)
	{
		if (__instance.Visuals is NHermitCreatureVisuals guardianVisuals)
			guardianVisuals.OnAnimationTrigger("Dead");
	}
}
