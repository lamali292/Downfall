using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Downfall.Code.History;

public class UnusedBlockEntry : CombatHistoryEntry
{
    public UnusedBlockEntry(
        int amount,
        Creature creature,
        int roundNumber,
        CombatSide currentSide,
        CombatHistory history)
        : base(creature, roundNumber, currentSide, history)
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

[HarmonyPatch(typeof(Creature), nameof(Creature.ClearBlock))]
internal static class ClearBlockPatch
{
    [HarmonyPrefix]
    private static bool SaveUnusedToHistory(Creature __instance)
    {
        var combatState = __instance.CombatState;
        if (combatState == null) return true;
        var entry = new UnusedBlockEntry(__instance.Block, __instance, combatState.RoundNumber, __instance.Side,
            CombatManager.Instance.History);
        CombatManager.Instance.History.Add(entry);
        return true;
    }
}