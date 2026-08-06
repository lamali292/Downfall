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
    private const float DeadMix = 0.05f;
    private MegaAnimationState? _animState;

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
            case "Dead":
                _animState?.SetAnimationWithMix("Dead", DeadMix, false);
                break;
            case "Attack":
            case "Cast":
                break;
        }
    }

    public override void _Ready()
    {
        base._Ready();
        _animState = SpineBody?.GetAnimationState();
        _animState?.SetAnimationCompat("Idle");
    }
}