using Godot;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Hexaghost.HexaghostCode.Vfx;

[GlobalClass]
public partial class NHexaghostVisuals : Node2D
{
    private AnimationNodeStateMachinePlayback? _playback;
    public override void _Ready()
    {
        var animTree = GetNode<AnimationTree>("%AnimationTree");
        animTree.Active = true;
        _playback = (AnimationNodeStateMachinePlayback)animTree.Get("parameters/playback");
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