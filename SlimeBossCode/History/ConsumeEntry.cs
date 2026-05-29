using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace SlimeBoss.SlimeBossCode.History;

public class ConsumeEntry : CombatHistoryEntry
{
    public ConsumeEntry(
        Creature goopedCreature,
        decimal goopAmount,
        Creature attacker,
        int roundNumber,
        CombatSide currentSide,
        CombatHistory history,
        IEnumerable<Player> players)
        : base(attacker, roundNumber, currentSide, history, players)
    {
        GoopedCreature = goopedCreature;
        GoopAmount = goopAmount;
    }

    public Creature GoopedCreature { get; }
    public decimal GoopAmount { get; }

    
    public override string Description => $"{GetId(Actor)} played Dead On effect for {GetId(Actor)}";

    private static string? GetId(Creature creature)
    {
        return creature.IsPlayer ? creature.Player?.Character.Id.Entry : creature.Monster?.Id.Entry;
    }
}