using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Interfaces;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Snecko.SneckoCode.Vfx;

[GlobalClass]
public partial class NSneckoCreatureVisuals : NCreatureVisuals, IAnimatedVisuals
{
    private const float DefaultMix = 0.2f;
    private const float ToIdleMix = 0.35f;
    private const float AttackMix = 0.1f;
    private const float CastMix = 0.1f;
    private const float HitMix = 0.05f;

    private const string CastAnim = "Attack";
    private const string IdleAnim = "Idle";
    private const string AttackAnim = "Attack_2";
    private const string HitAnim = "Hit";
    private MegaAnimationState? _animState;

    private MegaSprite? _sprite;

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

    public void OnAnimationTrigger(string trigger)
    {
        switch (trigger)
        {
            case "Idle":
                _animState?.SetAnimationWithMix(IdleAnim, DefaultMix);
                break;
            case "Attack":
                _animState?.SetAnimationWithMix(AttackAnim, AttackMix, loop: false);
                _animState?.QueueAnimation(IdleAnim, ToIdleMix);
                break;
            case "Hit":
                _animState?.SetAnimationWithMix(HitAnim, HitMix, loop: false);
                _animState?.QueueAnimation(IdleAnim, ToIdleMix);
                break;
            case "Cast":
                _animState?.SetAnimationWithMix(CastAnim, CastMix, loop: false);
                _animState?.QueueAnimation(IdleAnim, ToIdleMix);
                break;
            case "Dead":
                break;
        }
    }
}