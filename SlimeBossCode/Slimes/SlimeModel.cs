using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public abstract class SlimeModel : CustomMonsterModel
{
    public override int MinInitialHp => 1;
    public override int MaxInitialHp => 1;
    public override string CustomVisualPath =>  $"combat/{Id.Entry.RemovePrefix().ToLowerInvariant()}.tscn".SlimeScenePath();
    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var initialState = new MoveState("NOTHING_MOVE", _ => Task.CompletedTask);
        initialState.FollowUpState = initialState;
        return new MonsterMoveStateMachine([initialState], initialState);
    }
}