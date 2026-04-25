using Downfall.Code.Core.Hexaghost;
using Godot;

namespace Downfall.Code.Vfx.Hexaghost;

[GlobalClass]
public partial class NHexaghostVisuals : Node2D
{
    private NFire? _fire1;
    private NFire? _fire2;
    private NFire? _fire3;
    private NFire? _fire4;
    private NFire? _fire5;
    private NFire? _fire6;
    private Control? _fireNode;
    private AnimationNodeStateMachinePlayback? _playback;

    private Tween? _positionTween;
    private NFire?[] AllFires => [_fire1, _fire2, _fire3, _fire4, _fire5, _fire6];
    private IEnumerable<NFire> ValidFires => AllFires.OfType<NFire>();

    public override void _Ready()
    {
        _fireNode = GetNode<Control>("fire");

        _fire1 = GetNode<NFire>("%fire1");
        _fire2 = GetNode<NFire>("%fire2");
        _fire3 = GetNode<NFire>("%fire3");
        _fire4 = GetNode<NFire>("%fire4");
        _fire5 = GetNode<NFire>("%fire5");
        _fire6 = GetNode<NFire>("%fire6");

        var animTree = GetNode<AnimationTree>("%AnimationTree");
        animTree.Active = true;
        _playback = (AnimationNodeStateMachinePlayback)animTree.Get("parameters/playback");
    }

    private void SetFirePosition(int fireIndex, float duration = 0.5f)
    {
        if (_fireNode == null) return;
        _positionTween?.Kill();
        var targetRot = -(fireIndex - 0.5) * Mathf.Tau / 6f;
        var current = _fireNode.Rotation;
        var diff = Mathf.AngleDifference(current, targetRot);
        var newRot = current + diff;

        _positionTween = CreateTween().SetParallel();
        _positionTween.TweenProperty(_fireNode, "rotation", newRot, duration)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);

        foreach (var fire in AllFires)
            _positionTween.TweenProperty(fire, "rotation", -newRot, duration)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.InOut);
    }

    public void RefreshWheel(GhostflameModel[] wheel, int currentIndex)
    {
        for (var i = 0; i < wheel.Length; i++)
            AllFires[i]?.SetState(wheel[i].FireColor, wheel[i].IsIgnited ? NFire.FireSize.Large : NFire.FireSize.Small);
        SetFirePosition(currentIndex);
    }

    public void OnAnimationTrigger(string trigger)
    {
        if (_playback == null) return;
        var state = trigger switch
        {
            "Idle" => "idle",
            "Attack" => "attack",
            "Cast" => "cast",
            "Hit" => "hurt",
            "Dead" => "death",
            _ => "idle"
        };
        _playback.Travel(state);
    }
}