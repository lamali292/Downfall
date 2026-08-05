using Godot;

namespace Hexaghost.HexaghostCode.Vfx;

[GlobalClass]
public partial class NHexaghostVisuals : Node2D
{
	private static readonly StringName SpinParam = "shader_parameter/spin_speed";
	
	private const float InnerBase = 0.6f;
	private const float MiddleBase = 0.4f;
	private const float OuterBase = 0.275f;
	
	private const float SpinPerIgnited = 0.4f;

	private AnimationTree? _animTree;
	private AnimationNodeStateMachinePlayback? _playback;

	private ShaderMaterial? _innerSmoke;
	private ShaderMaterial? _middleSmoke;
	private ShaderMaterial? _outerSmoke;

	private int _ignitedCount;

	public override void _Ready()
	{
		_animTree = GetNode<AnimationTree>("AnimationTree");
		_animTree.Active = true;
		_playback = (AnimationNodeStateMachinePlayback)_animTree.Get("parameters/playback");

		var scene = GetNode<Node2D>("%HexaghostScene");
		_innerSmoke = GetSmokeMaterial(scene, "inner_smoke");
		_middleSmoke = GetSmokeMaterial(scene, "middle_smoke");
		_outerSmoke = GetSmokeMaterial(scene, "outer_smoke");
	}

	private static ShaderMaterial? GetSmokeMaterial(Node2D scene, string nodeName)
	{
		var node = scene.GetNodeOrNull<MeshInstance2D>(nodeName);
		return node?.Material as ShaderMaterial;
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
