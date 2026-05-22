using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Hermit.HermitCode.History;

public class DeadOnEntry : CombatHistoryEntry
{
    public DeadOnEntry(
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

    public override string Description => $"{GetId(Actor)} played Dead On effect for {CardPlay.Card.Id.Entry}";

    private static string? GetId(Creature creature)
    {
        return creature.IsPlayer ? creature.Player?.Character.Id.Entry : creature.Monster?.Id.Entry;
    }
}