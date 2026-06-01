using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public abstract class SlimeModel : CustomMonsterModel
{
    public override int MinInitialHp => 1;
    public override int MaxInitialHp => 1;
    public abstract SlimeType SlimeType { get; }

    public override string CustomVisualPath =>
        $"combat/{Id.Entry.RemovePrefix().ToLowerInvariant()}.tscn".SlimeScenePath();

    public override bool HasDeathSfx => false;

    public Creature PetOwner => Creature.PetOwner?.Creature ?? throw new ArgumentNullException(nameof(PetOwner));

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var initialState = new MoveState("NOTHING_MOVE", _ => Task.CompletedTask);
        initialState.FollowUpState = initialState;
        return new MonsterMoveStateMachine([initialState], initialState);
    }

    public abstract Task Command(PlayerChoiceContext ctx);
}

[Flags]
public enum SlimeType
{
    None = 0,
    Normal = 1,
    Specialist = 2,
    Any = Normal | Specialist
}