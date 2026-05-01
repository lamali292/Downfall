using Hexaghost.HexaghostCode.Vfx;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Hexaghost.HexaghostCode.Core;

public static class HexaghostVisualsBridge
{
    public static NHexaghostVisuals? GetVisuals(Player player)
    {
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
        return (creatureNode?.Visuals as NHexaghostCreatureVisuals)?.Visuals;
    }

    public static void Refresh(Player player)
    {
        var visuals = GetVisuals(player);
        if (visuals == null) return;
        var wheel = HexaghostCmd.GetWheel(player);
        var index = HexaghostCmd.GetCurrentIndex(player);
        visuals.RefreshWheel(wheel, index, player);
    }

    public static void RefreshCurrentIntent(Player player)
    {
        var visuals = GetVisuals(player);
        if (visuals == null) return;
        var wheel = HexaghostCmd.GetWheel(player);
        var index = HexaghostCmd.GetCurrentIndex(player);
        visuals.RefreshCurrentIntent(wheel, index, player);
    }
}