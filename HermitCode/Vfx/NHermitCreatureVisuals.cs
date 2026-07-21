using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Interfaces;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Hermit.HermitCode.Vfx;

[GlobalClass]
public partial class NHermitCreatureVisuals : NCreatureVisuals, IAnimatedVisuals
{
    private const float DefaultMix = 0.2f;
    private const float ToIdleMix = 0.35f;
    private const float AttackMix = 0.1f;
    private const float HitMix = 0.05f;
    private MegaAnimationState? _animState;

    private MegaSprite? _sprite;


    public void OnAnimationTrigger(string trigger)
    {
        switch (trigger)
        {
            case "Idle":
                _animState?.SetAnimationWithMix("Idle", DefaultMix);
                break;
            case "Hit":
                _animState?.SetAnimationWithMix("Hit", HitMix, false);
                _animState?.QueueAnimation("Idle", ToIdleMix);
                break;
            case "Attack":
            case "Cast":
            case "Dead":
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

        _sprite = SpineBody;
        _sprite?.SetNormalMaterial(premultMat);

        _animState = _sprite?.GetAnimationState();

        _animState?.SetAnimationCompat("Idle");
    }
}