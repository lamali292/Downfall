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
    private const float DeadMix = 0.35f;
    
    private MegaAnimationState? _animState;

    private MegaSprite? _sprite;

    
    private string IdleAnim =>"idle_loop";
    private string AttackAnim => "attack";
    private string CastAnim => "cast";
    private string HitAnim => "hurt";
    private string DeadAnim => "die";
    
    public void OnAnimationTrigger(string trigger)
    {
        switch (trigger)
        {
            case "Idle":
                _animState?.SetAnimationWithMix(IdleAnim, DefaultMix);
                break;
            case "Attack":
                _animState?.SetAnimationWithMix(AttackAnim, AttackMix, false);
                _animState?.QueueAnimation(IdleAnim, ToIdleMix);
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