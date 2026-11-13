using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace Downfall.DownfallCode.Vfx;

public abstract partial class NSpineMerchantCharacter : NMerchantCharacter
{
    protected abstract string IdleName { get; }

    public override void _Ready()
    {
        var megaTrackEntry = new MegaSprite((Variant)(GodotObject)GetChild(0));
        var premultMat = new CanvasItemMaterial
        {
            BlendMode = CanvasItemMaterial.BlendModeEnum.PremultAlpha
        };
        megaTrackEntry.SetNormalMaterial(premultMat);
        PlayAnimation(IdleName, true);
    }
}