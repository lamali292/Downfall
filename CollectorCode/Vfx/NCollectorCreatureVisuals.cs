using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Interfaces;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Collector.CollectorCode.Vfx;

[GlobalClass]
public partial class NCollectorCreatureVisuals : NCreatureVisuals, IAnimatedVisuals
{
    private const float DefaultMix = 0.2f;
    private const float ToIdleMix = 0.35f;
    private const float AttackMix = 0.1f;
    private const float CastMix = 0.1f;
    private const float HitMix = 0.05f;
    private const float DeadMix = 0.35f;

    private MegaAnimationState? _animState;
    private bool _eyeSetupDone;
    private Control? _leftEye;
    private MegaBone? _leftEyeBone;
    private Control? _rightEye;
    private MegaBone? _rightEyeBone;


    private string IdleAnim => "idle_loop";
    private string AttackAnim => "attack";
    private string CastAnim => "cast";
    private string HitAnim => "Hit";
    private string DeadAnim => "die";

    public void OnAnimationTrigger(string trigger)
    {
        switch (trigger)
        {
            case "Idle":
                _animState?.SetAnimationWithMix(IdleAnim, DefaultMix);
                break;
            case "Dead":
                _animState?.SetAnimationWithMix(DeadAnim, DeadMix, false);
                break;
            case "Attack":
            case "Cast":
                _animState?.SetAnimationWithMix(CastAnim, CastMix, false);
                _animState?.QueueAnimation(IdleAnim, ToIdleMix);
                break;
            case "Hit":
                break;
        }
    }
    
}