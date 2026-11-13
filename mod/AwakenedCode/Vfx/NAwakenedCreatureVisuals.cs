using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Awakened.AwakenedCode.Vfx;

public partial class NAwakenedCreatureVisuals : NCreatureVisuals
{
    private Node2D? _eyeFlare;
    private WingFlare? _wingFlare1, _wingFlare2, _wingFlare3, _wingFlare4;


    public override void _Ready()
    {
        base._Ready();
        var body = GetCurrentBody();
        _eyeFlare = body.GetNodeOrNull<Node2D>("%EyeFlare");
        _wingFlare1 = body.GetNodeOrNull<WingFlare>("%WingFlare1");
        _wingFlare2 = body.GetNodeOrNull<WingFlare>("%WingFlare2");
        _wingFlare3 = body.GetNodeOrNull<WingFlare>("%WingFlare3");
        _wingFlare4 = body.GetNodeOrNull<WingFlare>("%WingFlare4");
        SetParticles(false);
    }
    

    public void SetParticles(bool on)
    {
        SetFlare(_eyeFlare, on);
        _wingFlare1?.SetActive(on);
        _wingFlare2?.SetActive(on);
        _wingFlare3?.SetActive(on);
        _wingFlare4?.SetActive(on);
        
    }

    private static void SetFlare(Node2D? flare, bool on)
    {
        if (flare == null) return;
        foreach (var child in flare.GetChildren())
            if (child is GpuParticles2D p)
                p.Emitting = on;
    }
}