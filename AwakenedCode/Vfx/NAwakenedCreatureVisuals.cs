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
    private WingFlare? _wingFlare;

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
        _wingFlare = _body.GetNodeOrNull<WingFlare>("%WingFlare");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (_sprite == null) return;

        if (_eyeFlare != null)
        {
            var eye = _sprite.GetGlobalBoneTransformCompat("Eye");
            if (eye.HasValue)
                _eyeFlare.GlobalPosition = eye.Value.Origin;
        }

        if (_wingFlare != null)
        {
            var hips = _sprite.GetGlobalBoneTransformCompat("Hips");
            if (hips.HasValue)
                _wingFlare.GlobalPosition = hips.Value.Origin + new Vector2(WingPos.wingPosX, WingPos.wingPosY);
        }
    }

    private void SetParticles(bool on)
    {
        SetFlare(_eyeFlare, on);
        _wingFlare?.SetActive(on);
    }

    private static void SetFlare(Node2D? flare, bool on)
    {
        if (flare == null) return;
        foreach (var child in flare.GetChildren())
            if (child is GpuParticles2D p)
                p.Emitting = on;
    }
}

public static class WingPos
{
    public static float wingPosX => 10;
    public static float wingPosY => -20;
}