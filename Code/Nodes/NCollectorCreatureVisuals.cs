using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.Code.Nodes;

[GlobalClass]
public partial class NCollectorCreatureVisuals : NCreatureVisuals
{
    private bool _eyeSetupDone;
    private Control? _leftEye;
    private MegaBone? _leftEyeBone;
    private Control? _rightEye;
    private MegaBone? _rightEyeBone;

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