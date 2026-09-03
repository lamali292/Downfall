using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Interfaces;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Awakened.AwakenedCode.Vfx;

[GlobalClass]
public partial class NAwakenedCreatureVisuals : NCreatureVisuals, IAnimatedVisuals
{
    private const float DefaultMix = 0.2f;
    private const float ToIdleMix = 0.35f;
    private const float AttackMix = 0.1f;
    private const float CastMix = 0.1f;
    private const float HitMix = 0.05f;
    private const float DeadMix = 0.35f;

    private MegaAnimationState? _animState;

    private Node2D? _eyeFlare;

    private bool _isAwakened;
    private MegaSprite? _sprite;
    private WingFlare? _wingFlare1, _wingFlare2, _wingFlare3, _wingFlare4;

    public bool IsAwakened
    {
        get => _isAwakened;
        set
        {
            if (_isAwakened == value) return;
            _isAwakened = value;

            _animState?.SetAnimationWithMix(IdleAnim, 0.5f);
            SetParticles(value);
        }
    }

    private string IdleAnim => IsAwakened ? "idle_loop_awakened" : "idle_loop";
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
            case "Dead":
                _animState?.SetAnimationWithMix(DeadAnim, DeadMix, false);
                SetParticles(false);
                break;
            case "Cast":
                _animState?.SetAnimationWithMix(CastAnim, CastMix, false);
                _animState?.QueueAnimation(IdleAnim, ToIdleMix);
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

        _eyeFlare = _body.GetNodeOrNull<Node2D>("%EyeFlare");
        _wingFlare1 = _body.GetNodeOrNull<WingFlare>("%WingFlare1");
        _wingFlare2 = _body.GetNodeOrNull<WingFlare>("%WingFlare2");
        _wingFlare3 = _body.GetNodeOrNull<WingFlare>("%WingFlare3");
        _wingFlare4 = _body.GetNodeOrNull<WingFlare>("%WingFlare4");
        SetParticles(false);

    }
    

    private void SetParticles(bool on)
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