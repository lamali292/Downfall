using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Interfaces;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Champ.ChampCode.Vfx;

[GlobalClass]
public partial class NChampCreatureVisuals : NCreatureVisuals, IAnimatedVisuals
{
    public enum Stance
    {
        Normal,
        Berserker,
        Defensive,
        Ultimate
    }

    private const float DefaultMix = 0.2f;
    private const float ToIdleMix = 0.35f;
    private const float AttackMix = 0.1f;
    private const float DeathMix = 0.4f;
    private const float HitMix = 0.05f;
    private MegaAnimationState? _animState;
    private MegaSprite? _sprite;

    public Stance CurrentStance { get; set; } = Stance.Normal;

    private string IdleAnim => CurrentStance switch
    {
        Stance.Berserker => "idle_loop_berserker",
        Stance.Defensive => "idle_loop_defensive",
        Stance.Ultimate => "idle_loop_ultimate",
        _ => "idle_loop"
    };

    private string AttackAnim => "attack";

    private string DeathAnim => "die";


    private string HitAnim => CurrentStance switch
    {
        Stance.Berserker => "hurt_berserker",
        Stance.Defensive => "hurt_defensive",
        _ => "hurt"
    };


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
                break;
            case "jumpAttack":
                _animState?.SetAnimationWithMix("attack_jump", HitMix, false);
                _animState?.QueueAnimation(IdleAnim, ToIdleMix);
                break;
            case "Dead":
                _animState?.SetAnimationWithMix(DeathAnim, DeathMix, false);
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