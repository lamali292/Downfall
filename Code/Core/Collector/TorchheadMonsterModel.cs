using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Core.Collector;

public class TorchheadMonsterModel : CustomMonsterModel
{
    public override string CustomVisualPath =>
        "res://Downfall/character/scenes/combat_scene/collector/torchhead_combat.tscn";

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

    public override CreatureAnimator? SetupCustomAnimationStates(MegaSprite controller)
    {
        return SetupAnimationState(controller, "idle");
    }
}