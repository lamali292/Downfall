using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

namespace Downfall.Code.Core;

public abstract class GremlinsMonsterModel : CustomMonsterModel
{
    public override float DeathAnimLengthOverride => 0.2f;
    public override bool HasHurtSfx => false;
    public override bool HasDeathSfx => false;

    protected abstract string IdleAnimationName { get; }


    public override int MinInitialHp => 10;
    public override int MaxInitialHp => 10;

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var initialState = new MoveState("NOTHING_MOVE", _ => Task.CompletedTask);
        initialState.FollowUpState = initialState;
        return new MonsterMoveStateMachine([initialState], initialState);
    }

    public override CreatureAnimator? SetupCustomAnimationStates(MegaSprite controller)
    {
        return SetupAnimationState(controller, IdleAnimationName);
    }
}

public class AngryGremlin : GremlinsMonsterModel
{
    protected override string IdleAnimationName => "idle";

    public override string CustomVisualPath =>
        "res://Downfall/character/scenes/combat_scene/gremlins/angry_combat.tscn";
}

public class ShieldGremlin : GremlinsMonsterModel
{
    protected override string IdleAnimationName => "idle";

    public override string CustomVisualPath =>
        "res://Downfall/character/scenes/combat_scene/gremlins/shield_combat.tscn";
}

public class FatGremlin : GremlinsMonsterModel
{
    protected override string IdleAnimationName => "animation";
    public override string CustomVisualPath => "res://Downfall/character/scenes/combat_scene/gremlins/fat_combat.tscn";
}

public class SneakGremlin : GremlinsMonsterModel
{
    protected override string IdleAnimationName => "animation";

    public override string CustomVisualPath =>
        "res://Downfall/character/scenes/combat_scene/gremlins/sneak_combat.tscn";
}

public class WizardGremlin : GremlinsMonsterModel
{
    protected override string IdleAnimationName => "animation";

    public override string CustomVisualPath =>
        "res://Downfall/character/scenes/combat_scene/gremlins/wizard_combat.tscn";
}