using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Interfaces;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace SlimeBoss.SlimeBossCode.Vfx;

[GlobalClass]
public partial class NSlimeBossCreatureVisuals : NCreatureVisuals, IAnimatedVisuals
{
    private const float DefaultMix = 0.2f;
    private const float ToIdleMix = 0.35f;
    private const float AttackMix = 0.1f;
    private const float DeathMix = 0.4f;
    private const float HitMix = 0.05f;
    private MegaAnimationState? _animState;
    private MegaSprite? _sprite;
    
    public override void _Ready()
    {
        base._Ready();

        // Fix dark seams: atlas uses premultiplied alpha data,
        // so the spine sprite must use PremultAlpha blend mode
        var premultMat = new CanvasItemMaterial
        {
            BlendMode = CanvasItemMaterial.BlendModeEnum.PremultAlpha
        };
        
        
        _sprite = SpineBody;
        _sprite?.SetNormalMaterial(premultMat);

        _animState = _sprite?.GetAnimationState();

        if (_sprite != null)
            _sprite.SetNormalMaterial(premultMat);
        else
            GetCurrentBody().Material = premultMat;
        
        _animState?.SetAnimationCompat(IdleAnim);
    }
    
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
            case "Dead":
                _animState?.SetAnimationWithMix(DeadAnim, DeathMix, false);
                break;
            case "Attack":
            case "Hit":
            case "Cast":
                break;
        
        }
    }
}