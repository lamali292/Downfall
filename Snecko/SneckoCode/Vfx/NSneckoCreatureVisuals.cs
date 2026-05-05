using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Snecko.SneckoCode.Vfx;

[GlobalClass]
public partial class NSneckoCreatureVisuals : NCreatureVisuals
{
	
	private const float DefaultMix = 0.2f;
	private const float ToIdleMix = 0.35f;
	private const float AttackMix  = 0.1f;
	private const float CastMix  = 0.1f;
	private const float HitMix     = 0.05f;

	private MegaSprite?         _sprite;
	private MegaAnimationState? _animState;
	
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
				_animState?.SetAnimation(IdleAnim)
					?.SetMixDuration(DefaultMix);
				break;

			case "Cast":
				_animState?.SetAnimation(CastAnim, false)
					?.SetMixDuration(CastMix);
				_animState?.AddAnimation(IdleAnim)
					.SetMixDuration(ToIdleMix);
				break;
			case "Attack":
				_animState?.SetAnimation(AttackAnim, false)
					?.SetMixDuration(AttackMix);
				_animState?.AddAnimation(IdleAnim)
					.SetMixDuration(ToIdleMix);
				break;

			case "Hit":
				_animState?.SetAnimation(HitAnim, false)
					?.SetMixDuration(HitMix);
				_animState?.AddAnimation(IdleAnim)
					.SetMixDuration(ToIdleMix);
				break;

			case "Dead":
				break;
		}
	}

	private string CastAnim => "Attack";
	private string IdleAnim => "Idle";
	private string AttackAnim => "Attack_2";
	private string HitAnim => "Hit";
}


[HarmonyPatch(typeof(NCreature), nameof(NCreature.SetAnimationTrigger))]
public static class ChampAnimationPatch
{
	private static void Postfix(NCreature __instance, string trigger)
	{
		if (__instance.Visuals is NSneckoCreatureVisuals sneckoVisuals)
			sneckoVisuals.OnAnimationTrigger(trigger);
	}
}

[HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
public static class ChampDeathAnimPatch
{
	private static void Postfix(NCreature __instance)
	{
		if (__instance.Visuals is NSneckoCreatureVisuals sneckoVisuals)
			sneckoVisuals.OnAnimationTrigger("Dead");
	}
}
