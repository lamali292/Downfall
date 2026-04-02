using Downfall.Code.Nodes;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Downfall.Code.Commands;

public static class GremlinsCmd
{
    public static void SwitchGremlin(Creature creature, int index)
    {
        if (NCombatRoom.Instance == null) return;
        var creatureNode = NCombatRoom.Instance.GetCreatureNode(creature);
        if (creatureNode?.Visuals is not NGremlinsCreatureVisuals visuals) return;
        visuals.SwitchToGremlin(index);
    }

    public static void KillGremlin(Creature creature, int index)
    {
        if (NCombatRoom.Instance == null) return;
        var creatureNode = NCombatRoom.Instance.GetCreatureNode(creature);
        if (creatureNode?.Visuals is not NGremlinsCreatureVisuals visuals) return;
        visuals.KillGremlin(index);
    }
}