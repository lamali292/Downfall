using Godot;
using Hexaghost.HexaghostCode.Vfx;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Hexaghost.HexaghostCode.Core;

public static class HexaghostVisualsBridge
{
    private static readonly Dictionary<Player, NGhostflames> Displays = new();

    public static NGhostflames? GetVisuals(Player player)
    {
        var display = Displays.GetValueOrDefault(player);
        if (GodotObject.IsInstanceValid(display))
            return display;
        Displays.Remove(player);   // stale entry from a previous combat
        return null;
    }

    public static void DiscardDisplay(Player player)
    {
        if (Displays.TryGetValue(player, out var old) && GodotObject.IsInstanceValid(old))
            old.QueueFree();
        Displays.Remove(player);
    }
    
    public static void Setup(NCombatRoom combatRoom, Player player)
    {
        if (Displays.TryGetValue(player, out var old) && GodotObject.IsInstanceValid(old))
            old.QueueFree();

        var scene = ResourceLoader.Load<PackedScene>(GhostflamesScenePath);
        var display = scene.Instantiate<NGhostflames>();

        var vfxContainer = combatRoom.CombatVfxContainer;
        vfxContainer.AddChildSafely(display);

        var creatureNode = combatRoom.GetCreatureNode(player.Creature);
        if (creatureNode != null)
            display.Track(creatureNode, vfxContainer);

        Displays[player] = display;
        Refresh(player);
    }

    private const string GhostflamesScenePath = "res://Hexaghost/scenes/ui/ghostflames.tscn"; // adjust path

    public static void Refresh(Player player)
    {
        var visuals = GetVisuals(player);
        if (visuals == null)
        {
            if (player.Character is not Hexaghost) return;
            if (NCombatRoom.Instance is not { } room) return;
            Setup(room, player);
            visuals = GetVisuals(player);
            if (visuals == null) return;
        }
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