using Downfall.DownfallCode.Core;
using Godot;
using Guardian.GuardianCode.Vfx;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Guardian.GuardianCode.Displays;

public class GuardianDisplay
{
    private static readonly PlayerField<NGuardianDisplay> Displays = new(() => null);

    public static bool HasDisplay(Player player) => GodotObject.IsInstanceValid(Displays[player]);

    public static void Refresh(Player creature)
    {
        var display = Displays[creature];
        if (GodotObject.IsInstanceValid(display))
            display!.Refresh();
        else if (display != null)
            Displays[creature] = null;
    }

    public static void RefreshCounters(Player creature)
    {
        var display = Displays[creature];
        if (GodotObject.IsInstanceValid(display))
            display!.RefreshCounters();
        else if (display != null)
            Displays[creature] = null;
    }

    private static void Register(Player creature, NGuardianDisplay display)
    {
        var old = Displays[creature];
        if (GodotObject.IsInstanceValid(old))
            old!.QueueFree();

        Displays[creature] = display;
    }

    public static NCard? GetNCard(CardModel card)
    {
        var display = Displays[card.Owner];
        return GodotObject.IsInstanceValid(display) ? display!.GetNCard(card) : null;
    }

    public static Vector2? GetPosition(CardModel model)
    {
        var display = Displays[model.Owner];
        return GodotObject.IsInstanceValid(display) ? display!.GetTargetPosition(model) : null;
    }

    public static void SetupGuardianUi(NCombatRoom combatRoom, Player player)
    {
        var creatureNode = combatRoom.GetCreatureNode(player.Creature);
        var display = NGuardianDisplay.Create(player, creatureNode?.Hitbox);
        var vfxContainer = combatRoom.CombatVfxContainer;
        vfxContainer.AddChildSafely(display);

        if (creatureNode != null)
        {
            var globalTopPos = creatureNode.GetTopOfHitbox();
            display.Position = vfxContainer.GetGlobalTransform().AffineInverse() * globalTopPos;
            display.Position += new Vector2(0f, -120f);
        }

        Register(player, display);
    }
}