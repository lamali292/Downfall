using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Interfaces;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Automaton.AutomatonCode.Vfx;

[GlobalClass]
public partial class NAutomatonCreatureVisuals : NCreatureVisuals, IAnimatedVisuals
{

    private MegaAnimationState? _animState;
    private MegaSprite? _sprite;
    private const float DefaultMix = 0.2f;
    private const float ToIdleMix = 0.35f;
    private const float AttackMix = 0.1f;
    private const float CastMix = 0.1f;
    private const float HitMix = 0.05f;
    private const float DeadMix = 0.35f;

    private const string HitAnim = "hit";
    private const string CastAnim = "cast";
    private const string DeadAnim = "dead";
    private string IdleAnim => "idle";

    public void OnAnimationTrigger(string trigger)
    {
        switch (trigger)
        {
            case "Idle":
                _animState?.SetAnimationWithMix(IdleAnim, DefaultMix);
                break;
            case "Attack":
                break;
            case "Hit":
                _animState?.SetAnimationWithMix(HitAnim, HitMix, false);
                _animState?.QueueAnimation(IdleAnim, ToIdleMix);
                break;
            case "Cast":
                _animState?.SetAnimationWithMix(CastAnim, CastMix, false);
                _animState?.QueueAnimation(IdleAnim, ToIdleMix);
                break;
            case "Dead":
                _animState?.SetAnimationWithMix(DeadAnim, DeadMix, false);
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