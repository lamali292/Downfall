using Downfall.DownfallCode.Interfaces;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Champ.ChampCode.Vfx;

[GlobalClass]
public partial class NChampCreatureVisuals : NCreatureVisuals, IAnimatedVisuals
{
	public enum Stance { Normal, Berserker, Defensive, Ultimate }

	private const float DefaultMix = 0.2f;
	private const float ToIdleMix = 0.35f;
	private const float AttackMix  = 0.1f;
	private const float HitMix     = 0.05f;

	private MegaSprite?         _sprite;
	private MegaAnimationState? _animState;
	private Stance             _currentStance = Stance.Normal;
	
	public Stance CurrentStance
	{
		get => _currentStance;
		set => _currentStance = value;
	}

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
				// TODO: add Death animation if one exists in the skeleton
				break;
		}
	}

	private string IdleAnim => _currentStance switch
	{
		Stance.Berserker => "IdleBerserker",
		Stance.Defensive => "IdleDefensive",
		Stance.Ultimate => "IdleUltimate",
		_                => "Idle"
	};

	private string AttackAnim => _currentStance switch
	{
		_                => "Attack"
	};

	private string HitAnim => _currentStance switch
	{
		Stance.Berserker => "HitBerserker",
		Stance.Defensive => "HitDefensive",
		_                => "Hit"
	};
}