using System.Numerics;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Interfaces;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Vfx;

namespace SlimeBoss.SlimeBossCode.Slimes;

public abstract class SlimeModel : CustomMonsterModel
{
    public override int MinInitialHp => 1;
    public override int MaxInitialHp => 1;

    public override string CustomVisualPath =>
        $"combat/{Id.Entry.RemovePrefix().ToLowerInvariant()}.tscn".SlimeScenePath();

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var initialState = new MoveState("NOTHING_MOVE", _ => Task.CompletedTask);
        initialState.FollowUpState = initialState;
        return new MonsterMoveStateMachine([initialState], initialState);
    }

    public override bool HasDeathSfx => false;

}