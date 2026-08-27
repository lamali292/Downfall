using Downfall.DownfallCode.Core;
using Godot;
using Hexaghost.HexaghostCode.Vfx;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace Hexaghost.HexaghostCode.Core;

public static class HexaghostVisualsBridge
{
    private static readonly PlayerField<NGhostflames> Displays = new(() => null);

    public static NGhostflames? GetVisuals(Player? player)
    {
        var display = Displays[player];
        if (GodotObject.IsInstanceValid(display))
            return display;
        if (display != null)
            Displays[player] = null;
        return null;
    }

    public static void FadeFlamesOnDeath(Player player)
    {
        GetVisuals(player)?.FadeOutOnDeath();
    }

    public static void FadeFlamesOnRevive(Player player)
    {
        GetVisuals(player)?.FadeInOnRevive();
    }

    private static void Setup(NCombatRoom combatRoom, Player player)
    {
        var existing = Displays[player];
        if (GodotObject.IsInstanceValid(existing))
        {
            HexaghostMainFile.Logger.Info(
                $"[Ghostflames] Setup: freeing previous display #{existing!.GetInstanceId()} for player");
            existing.QueueFree();
        }

        var display = NGhostflames.Create(player);
        Displays[player] = display;

        var vfxContainer = combatRoom.CombatVfxContainer;

        vfxContainer.AddChildSafely(display);

        var creatureNode = combatRoom.GetCreatureNode(player.Creature);
        if (creatureNode != null)
            display.Track(creatureNode, vfxContainer);
        else
            HexaghostMainFile.Logger.Warn(
                "[Ghostflames] Setup: creature node not found; flames won't track until next Refresh");

        Refresh(player);
    }

    public static void Refresh(Player player)
    {
        if (TestMode.IsOn) return;
        var visuals = GetVisuals(player);
        if (visuals == null)
        {
            if (NCombatRoom.Instance is not { } room)
            {
                HexaghostMainFile.Logger.Warn("[Ghostflames] Refresh: no combat room, skipping");
                return;
            }

            Setup(room, player);
            return;
        }

        var wheel = HexaghostCmd.GetWheel(player);
        var index = HexaghostCmd.GetCurrentIndex(player);
        visuals.RefreshWheel(wheel, index);

        var ignited = wheel.Count(f => f.IsIgnited);
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
        var bodyVisuals = creatureNode?.GetSpecialNode<NHexaghostVisuals>("%Hexaghost");
        bodyVisuals?.SetIgnitedCount(ignited);
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