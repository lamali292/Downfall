using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Snecko.SneckoCode.History;

public class MuddleEntry(
    CardModel card,
    Creature creature,
    int roundNumber,
    CombatSide currentSide,
    CombatHistory history,
    IEnumerable<Player> players)
    : CombatHistoryEntry(creature, roundNumber, currentSide, history, players)
{
    public CardModel Card { get; } = card;

    public override string Description => $"{GetId(Actor)} muddled {Card.Id.Entry}";

    private static string? GetId(Creature creature)
    {
        return creature.IsPlayer ? creature.Player?.Character.Id.Entry : creature.Monster?.Id.Entry;
    }
}