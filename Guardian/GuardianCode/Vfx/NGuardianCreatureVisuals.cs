using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Guardian.GuardianCode.Vfx;

[GlobalClass]
public partial class NGuardianCreatureVisuals : NCreatureVisuals
{
    public override void _Ready()
    {
        base._Ready();

        // Fix dark seams: atlas uses premultiplied alpha data,
        // so the spine sprite must use PremultAlpha blend mode
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
