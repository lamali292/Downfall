using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using Hexaghost.HexaghostCode.Core;

namespace Hexaghost.HexaghostCode.History;

/// <summary>
///     A Ghostflame was ignited by hand during a card's play (<c>HexaghostCmd.IgniteAt</c>).
///     <see cref="WasPrimedBySource" /> records whether the same play would also have ignited that flame once it
///     resolved, so the flame can still ignite a second time after the play.
/// </summary>
public class GhostflameIgniteEntry(
    GhostflameModel flame,
    CardModel source,
    bool wasPrimedBySource,
    Creature creature,
    int roundNumber,
    CombatSide currentSide,
    CombatHistory history,
    IEnumerable<Player> players)
    : CombatHistoryEntry(creature, roundNumber, currentSide, history, players)
{
    public GhostflameModel Flame { get; } = flame;
    public CardModel Source { get; } = source;
    public bool WasPrimedBySource { get; } = wasPrimedBySource;

    public override string Description => $"{Source.Id.Entry} ignited {Flame.Id.Entry}";
}
