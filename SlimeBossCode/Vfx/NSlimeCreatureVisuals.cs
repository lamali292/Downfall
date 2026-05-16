using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace SlimeBoss.SlimeBossCode.Vfx;

[GlobalClass]
public partial class NSlimeCreatureVisuals  : NCreatureVisuals
{
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
    }
}