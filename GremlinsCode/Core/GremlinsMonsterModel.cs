using BaseLib.Abstracts;
using Downfall.DownfallCode.Powers;
using Gremlins.GremlinsCode.Powers;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gremlins.GremlinsCode.Core;

public abstract class GremlinsMonsterModel : CustomMonsterModel
{
    public override float DeathAnimLengthOverride => 0.2f;
    public override bool HasHurtSfx => false;
    public override bool HasDeathSfx => false;

    protected abstract string IdleAnimationName { get; }


    public override int MinInitialHp => 16;
    public override int MaxInitialHp => 16;

    public virtual bool ShouldSave => true;

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var initialState = new MoveState("NOTHING_MOVE", _ => Task.CompletedTask);
        initialState.FollowUpState = initialState;
        return new MonsterMoveStateMachine([initialState], initialState);
    }

    public override CreatureAnimator SetupCustomAnimationStates(MegaSprite controller)
    {
        return SetupAnimationState(controller, IdleAnimationName);
    }

    public virtual Task TriggerGremlinBonus(PlayerChoiceContext ctx, Player player)
    {
        return Task.CompletedTask;
    }
}

public class MadGremlin : GremlinsMonsterModel
{
    protected override string IdleAnimationName => "idle";

    public override string CustomVisualPath =>
        "res://Gremlins/scenes/gremlins/angry/angry_combat.tscn";

    public override async Task TriggerGremlinBonus(PlayerChoiceContext ctx, Player player)
    {
        await PowerCmd.Apply<MadGremlinPower>(ctx, player.Creature, 2, player.Creature, null);
    }
}
public class MadGremlinPower : CustomTemporaryPowerModelWrapper<MadGremlin, StrengthPower>;


public class ShieldGremlin : GremlinsMonsterModel
{
    protected override string IdleAnimationName => "idle";

    public override string CustomVisualPath =>
        "res://Gremlins/scenes/gremlins/shield/shield_combat.tscn";

    public override async Task TriggerGremlinBonus(PlayerChoiceContext ctx, Player player)
    {
        await CreatureCmd.GainBlock(player.Creature, 2, ValueProp.Unpowered, null);
    }
}

public class FatGremlin : GremlinsMonsterModel
{
    protected override string IdleAnimationName => "animation";

    public override string CustomVisualPath =>
        "res://Gremlins/scenes/gremlins/fat/fat_combat.tscn";

    public override async Task TriggerGremlinBonus(PlayerChoiceContext ctx, Player player)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null) return;
        await PowerCmd.Apply<WeakPower>(ctx, combatState.HittableEnemies, 1, player.Creature, null);
    }
}

public class SneakGremlin : GremlinsMonsterModel
{
    protected override string IdleAnimationName => "animation";

    public override string CustomVisualPath =>
        "res://Gremlins/scenes/gremlins/sneak/sneak_combat.tscn";


    public override async Task TriggerGremlinBonus(PlayerChoiceContext ctx, Player player)
    {
        var combatState = player.Creature.CombatState;
        var randomEnemy = combatState?.RunState.Rng.CombatTargets.NextItem(combatState.HittableEnemies);
        if (randomEnemy == null) return;
        await CreatureCmd.Damage(ctx, randomEnemy, 2, ValueProp.Unpowered, player.Creature);
    }
}

public class WizardGremlin : GremlinsMonsterModel
{
    protected override string IdleAnimationName => "animation";

    public override string CustomVisualPath =>
        "res://Gremlins/scenes/gremlins/wizard/wizard_combat.tscn";

    public override async Task TriggerGremlinBonus(PlayerChoiceContext ctx, Player player)
    {
        await PowerCmd.Apply<WizPower>(ctx, player.Creature, 1, player.Creature, null);
    }
}

public class GremlinNob : GremlinsMonsterModel
{
    protected override string IdleAnimationName => "animation";

    public override string CustomVisualPath =>
        "res://Gremlins/scenes/gremlins/nob/nob_combat.tscn";

    public override bool ShouldSave => false;
}