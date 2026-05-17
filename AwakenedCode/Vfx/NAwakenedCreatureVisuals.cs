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
    private const float HitMix = 0.05f;
    private MegaAnimationState? _animState;

    private MegaSprite? _sprite;

    public bool IsAwakened { get; set; }

    private string IdleAnim => IsAwakened ? "Idle_2" : "Idle_1";
    private string AttackAnim => IsAwakened ? "Attack_2" : "Attack_1";

    public void OnAnimationTrigger(string trigger)
    {
        switch (trigger)
        {
            case "Idle":
                _animState?.SetAnimation(IdleAnim)
                    ?.SetMixDuration(DefaultMix);
                break;

            case "Cast":
                break;

            case "Attack":
                _animState?.SetAnimation(AttackAnim, false)
                    ?.SetMixDuration(AttackMix);
                _animState?.AddAnimation(IdleAnim)
                    .SetMixDuration(ToIdleMix);
                break;

            case "Hit":
                _animState?.SetAnimation("Hit", false)
                    ?.SetMixDuration(HitMix);
                _animState?.AddAnimation(IdleAnim)
                    .SetMixDuration(ToIdleMix);
                break;

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

        _animState?.SetAnimation("Idle_1");
    }
}