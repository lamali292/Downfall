using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Core;

public class TorchheadMonsterModel : CustomMonsterModel
{
    public override string CustomVisualPath =>
        "res://Collector/scenes/character/torchhead_combat.tscn";

    public override int MinInitialHp => 1;
    public override int MaxInitialHp => 1;

    public override float DeathAnimLengthOverride => 0.2f;
    public override bool HasHurtSfx => false;
    public override bool HasDeathSfx => false;

    public override bool IsHealthBarVisible => Creature.IsAlive;

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var initialState = new MoveState("NOTHING_MOVE", _ => Task.CompletedTask);
        initialState.FollowUpState = initialState;
        return new MonsterMoveStateMachine([initialState], initialState);
    }

    public override async Task AfterDamageReceivedLate(PlayerChoiceContext choiceContext, Creature target,
        DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target != Creature) return;
        await CreatureCmd.SetMaxHp(target, Creature.CurrentHp);
    }

    public override CreatureAnimator SetupCustomAnimationStates(MegaSprite controller)
    {
        var idleState = new AnimState("idle_loop", true);
        var castState = new AnimState("cast");
        var attackState = new AnimState("attack");
        var hurtState = new AnimState("hurt");
        var dieState = new AnimState("die");
        //var deadLoopState = new AnimState("dead_loop", true);
        var reviveState = new AnimState("revive");
        idleState.AddBranch("Hit", hurtState);
        castState.NextState = idleState;
        castState.AddBranch("Hit", hurtState);
        attackState.NextState = idleState;
        attackState.AddBranch("Hit", hurtState);
        hurtState.NextState = idleState;
        hurtState.AddBranch("Hit", hurtState);
        //dieState.NextState = deadLoopState;
        reviveState.NextState = idleState;
        var animator = new CreatureAnimator(idleState, controller);
        animator.AddAnyState("Attack", attackState);
        animator.AddAnyState("Cast", castState);
        animator.AddAnyState("Dead", dieState);
        animator.AddAnyState("Revive", reviveState);
        return animator;
    }
}