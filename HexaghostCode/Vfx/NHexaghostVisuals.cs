using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace Hexaghost.HexaghostCode.Vfx;

[GlobalClass]
public partial class NHexaghostVisuals : Node2D
{
	private const float InnerBase = 0.6f;
	private const float MiddleBase = 0.4f;
	private const float OuterBase = 0.275f;

	private const float SpinPerIgnited = 0.4f;

	private int _ignitedCount;

	private ShaderMaterial? _innerSmoke;
	private ShaderMaterial? _middleSmoke;
	private ShaderMaterial? _outerSmoke;

	private AnimationPlayer? _glowPlayer;
	private MegaSprite? _sprite;

	public override void _Ready()
	{
		_innerSmoke = MakeUniqueMaterial<MeshInstance2D>("%inner_smoke");
		_middleSmoke = MakeUniqueMaterial<MeshInstance2D>("%middle_smoke");
		_outerSmoke = MakeUniqueMaterial<MeshInstance2D>("%outer_smoke");

		_glowPlayer = GetNodeOrNull<AnimationPlayer>("GlowAnimationPlayer");

		// This node's underlying native class is SpineSprite (the script is just attached on top of it).
		_sprite = new MegaSprite(this);

		// "animation_started" passes 3 native args here (undocumented in our source tree, and not
		// necessarily 1 as MegaSprite's own C# wrapper methods assume) - typing them as Variant sidesteps
		// needing to know what they actually are, since we just re-read the current animation ourselves.
		_sprite.ConnectAnimationStarted(Callable.From<Variant, Variant, Variant>((_, _, _) => SyncGlowAnimation()));
	}

	/// <summary>
	/// The core glow shader pulse and the particle bursts aren't part of the Spine rig - keep them in a
	/// small local AnimationPlayer and play it under the same name Spine just switched to
	/// (idle_loop/attack/cast/hurt/die). No-ops for any name with no matching glow track.
	/// </summary>
	private void SyncGlowAnimation()
	{
		var name = _sprite?.TryGetAnimationState()?.GetCurrentAnimationName();
		if (name != null && _glowPlayer != null && _glowPlayer.HasAnimation(name))
			_glowPlayer.Play(name);
	}

	private ShaderMaterial? MakeUniqueMaterial<T>(string uniquePath) where T : CanvasItem
	{
		var node = GetNodeOrNull<T>(uniquePath);
		if (node?.Material is not ShaderMaterial shared)
			return null;

		var unique = (ShaderMaterial)shared.Duplicate();
		node.Material = unique;
		return unique;
	}

	/// <summary>Call whenever the wheel changes; count = number of ignited flames (0..6).</summary>
	public void SetIgnitedCount(int count)
	{
		_ignitedCount = count;
		ApplySpin();
	}

	private void ApplySpin()
	{
		var boost = _ignitedCount * SpinPerIgnited;
		_innerSmoke?.SetShaderParameter("spin_speed", InnerBase + boost);
		_middleSmoke?.SetShaderParameter("spin_speed", MiddleBase + boost);
		_outerSmoke?.SetShaderParameter("spin_speed", OuterBase + boost);
	}
}
