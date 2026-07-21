using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Interfaces;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Automaton.AutomatonCode.Vfx;

[GlobalClass]
public partial class NAutomatonCreatureVisuals : NCreatureVisuals, IAnimatedVisuals
{
    private const float DefaultMix = 0.2f;
    private MegaAnimationState? _animState;
    private MegaSprite? _sprite;
    private string IdleAnim => "idle";

    public void OnAnimationTrigger(string trigger)
    {
        switch (trigger)
        {
            case "Idle":
                _animState?.SetAnimationWithMix(IdleAnim, DefaultMix);
                break;
            case "Attack":
            case "Hit":
            case "Cast":
            case "Dead":
                break;
        }
    }

    public override void _Ready()
    {
        base._Ready();

        base._Ready();

        var premultMat = new CanvasItemMaterial
        {
            BlendMode = CanvasItemMaterial.BlendModeEnum.PremultAlpha
        };

        _sprite = SpineBody;
        _sprite?.SetNormalMaterial(premultMat);

        _animState = _sprite?.GetAnimationState();

        _animState?.SetAnimationCompat(IdleAnim);
    }
}