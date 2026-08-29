using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Interfaces;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Collector.CollectorCode.Vfx;

[GlobalClass]
public partial class NCollectorCreatureVisuals : NCreatureVisuals, IAnimatedVisuals
{
    private const float DefaultMix = 0.2f;
    private const float ToIdleMix = 0.35f;
    private const float AttackMix = 0.1f;
    private const float CastMix = 0.1f;
    private const float HitMix = 0.05f;
    private const float DeadMix = 0.35f;

    private MegaAnimationState? _animState;
    private bool _eyeSetupDone;
    private Control? _leftEye;
    private MegaBone? _leftEyeBone;
    private Control? _rightEye;
    private MegaBone? _rightEyeBone;


    private string IdleAnim => "idle";
    private string AttackAnim => "attack";
    private string CastAnim => "cast";
    private string HitAnim => "Hit";
    private string DeadAnim => "die";

    public void OnAnimationTrigger(string trigger)
    {
        switch (trigger)
        {
            case "Idle":
                _animState?.SetAnimationWithMix(IdleAnim, DefaultMix);
                break;
            case "Hit":
                _animState?.SetAnimationWithMix(HitAnim, HitMix, false);
                _animState?.QueueAnimation(IdleAnim, ToIdleMix);
                break;
            case "Attack":
            case "Dead":
            case "Cast":
                break;
        }
    }

    public override void _Ready()
    {
        base._Ready();

        var premultMat = new CanvasItemMaterial
        {
            BlendMode = CanvasItemMaterial.BlendModeEnum.PremultAlpha
        };

        if (SpineBody != null)
            SpineBody.SetNormalMaterial(premultMat);
        else
            GetCurrentBody().Material = premultMat;

        GetTree().ProcessFrame += SetupEyes;
    }

    private void SetupEyes()
    {
        if (_eyeSetupDone) return;
        _eyeSetupDone = true;
        GetTree().ProcessFrame -= SetupEyes;

        _animState = SpineBody?.GetAnimationState();
        _animState?.SetAnimationCompat(IdleAnim);

        _rightEye = GetNodeOrNull<Control>("Visuals/RightEye");
        _leftEye = GetNodeOrNull<Control>("Visuals/LeftEye");

        if (SpineBody == null) return;

        var skeleton = SpineBody.GetSkeleton();
        _rightEyeBone = skeleton?.FindBone("righteyefireslot");
        _leftEyeBone = skeleton?.FindBone("lefteyefireslot");

        if (_rightEyeBone == null) GD.PrintErr("[Collector] righteyefireslot bone not found!");
        if (_leftEyeBone == null) GD.PrintErr("[Collector] lefteyefireslot bone not found!");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        UpdateEyePositions();
    }

    private void UpdateEyePositions()
    {
        if (SpineBody?.BoundObject is not Node2D spineNode) return;
        UpdateEye(_rightEye, "righteyefireslot", spineNode);
        UpdateEye(_leftEye, "lefteyefireslot", spineNode);
    }

    private void UpdateEye(Control? eye, string boneName, Node2D spineNode)
    {
        if (eye == null) return;
        var skeleton = SpineBody!.GetSkeleton();
        var bone = skeleton?.FindBone(boneName);
        if (bone == null) return;

        var wx = bone.BoundObject.Call("get_world_x").As<float>();
        var wy = bone.BoundObject.Call("get_world_y").As<float>();
        eye.Position = new Vector2(wx * 0.7f + 52, wy - 60);
    }
}