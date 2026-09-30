using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Champ.ChampCode.History;

/// <summary>Recorded once per Finisher card play, whether or not the player was in a stance with a Finisher effect.</summary>
public class FinisherEntry : CombatHistoryEntry
{
    public FinisherEntry(
        CardPlay cardPlay,
        Creature creature,
        int roundNumber,
        CombatSide currentSide,
        CombatHistory history,
        IEnumerable<Player> players)
        : base(creature, roundNumber, currentSide, history, players)
    {
        CardPlay = cardPlay;
    }

    public CardPlay CardPlay { get; }

    /// <summary>True if <paramref name="player"/> has no Finisher recorded yet this turn.</summary>
    public static bool IsFirstThisTurn(Player player)
    {
        var combatState = player.Creature.CombatState;
        return !CombatManager.Instance.History.Entries.OfType<FinisherEntry>()
            .Any(e => e.Actor == player.Creature && e.HappenedThisTurn(combatState));
    }

    public override string Description => $"{Actor.Player?.Character.Id.Entry} played Finisher {CardPlay.Card.Id.Entry}";
}
