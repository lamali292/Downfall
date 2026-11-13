using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.DownfallCode.History;

public class UnusedBlockEntry : CombatHistoryEntry
{
    public UnusedBlockEntry(
        int amount,
        Creature creature,
        int roundNumber,
        CombatSide currentSide,
        CombatHistory history,
        IEnumerable<Player> players)
        : base(creature, roundNumber, currentSide, history, players)
    {
        Amount = amount;
    }

    public int Amount { get; }

    public override string Description => $"{GetId(Actor)} didnt use {Amount} block";

    private static string? GetId(Creature creature)
    {
        return !creature.IsPlayer ? creature.Monster?.Id.Entry : creature.Player?.Character.Id.Entry;
    }
}