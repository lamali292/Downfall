using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.History;

/// <summary>
///     A Ghostwheel move (<see cref="WheelAdvanceEntry" /> / <see cref="WheelRetractEntry" />), recorded by
///     <c>HexaghostCmd.Advance</c> / <c>Retract</c> before the after-move hooks fire.
/// </summary>
public abstract class WheelMoveEntry(
    AbstractModel? source,
    Creature creature,
    int roundNumber,
    CombatSide currentSide,
    CombatHistory history,
    IEnumerable<Player> players)
    : CombatHistoryEntry(creature, roundNumber, currentSide, history, players)
{
    public AbstractModel? Source { get; } = source;

    protected string? ActorId => Actor.IsPlayer ? Actor.Player?.Character.Id.Entry : Actor.Monster?.Id.Entry;
}

public class WheelAdvanceEntry(
    AbstractModel? source,
    bool isEndOfTurnAdvance,
    Creature creature,
    int roundNumber,
    CombatSide currentSide,
    CombatHistory history,
    IEnumerable<Player> players)
    : WheelMoveEntry(source, creature, roundNumber, currentSide, history, players)
{
    /// <summary>The automatic advance off an ignited flame at end of turn, not an Advance the player performed.</summary>
    public bool IsEndOfTurnAdvance { get; } = isEndOfTurnAdvance;

    public override string Description => $"{ActorId} advanced the Ghostwheel";
}

public class WheelRetractEntry(
    AbstractModel? source,
    Creature creature,
    int roundNumber,
    CombatSide currentSide,
    CombatHistory history,
    IEnumerable<Player> players)
    : WheelMoveEntry(source, creature, roundNumber, currentSide, history, players)
{
    public override string Description => $"{ActorId} retracted the Ghostwheel";
}
