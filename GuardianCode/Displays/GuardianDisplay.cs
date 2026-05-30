using Godot;
using Guardian.GuardianCode.Vfx;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Guardian.GuardianCode.Displays;

public class GuardianDisplay
{
    private static readonly Dictionary<Player, NGuardianDisplay> Displays = new();

    static GuardianDisplay()
    {
        CombatManager.Instance.CombatEnded += _ =>
        {
            foreach (var d in Displays.Values)
                d.QueueFree();
            Displays.Clear();
        };
    }

    public static bool HasDisplay(Player player)
    {
        return Displays.TryGetValue(player, out var display) && GodotObject.IsInstanceValid(display);
    }

    public static void Refresh(Player creature)
    {
        Displays.GetValueOrDefault(creature)?.Refresh();
    }

    public static void RefreshCounters(Player creature)
    {
        Displays.GetValueOrDefault(creature)?.RefreshCounters();
    }


    public static void Register(Player creature, NGuardianDisplay display)
    {
        if (Displays.TryGetValue(creature, out var old))
            if (GodotObject.IsInstanceValid(old))
                old.QueueFree();

        Displays[creature] = display;
    }

    public static NCard? GetNCard(CardModel card)
    {
        Displays.TryGetValue(card.Owner, out var display);
        return display?.GetNCard(card) ?? null;
    }

    public static Vector2? GetPosition(CardModel model)
    {
        Displays.TryGetValue(model.Owner, out var display);
        return display?.GetTargetPosition(model) ?? null;
    }

    public static void SetupGuardianUi(NCombatRoom combatRoom, Player player)
    {
        var display = NGuardianDisplay.Create(player);
        var vfxContainer = combatRoom.CombatVfxContainer;
        vfxContainer.AddChildSafely(display);

        var creatureNode = combatRoom.GetCreatureNode(player.Creature);
        if (creatureNode != null)
        {
            var globalTopPos = creatureNode.GetTopOfHitbox();
            display.Position = vfxContainer.GetGlobalTransform().AffineInverse() * globalTopPos;
            display.Position += new Vector2(0f, -120f);
        }

        Register(player, display);
    }

    /*
    public static async Task AnimateCardToStasis(CardModel card, GuardianPile pile, Player creature)
    {
        var display = Displays.GetValueOrDefault(creature);
        if (display == null) return;

        var vfx = NCombatRoom.Instance?.CombatVfxContainer;
        if (vfx == null) return;

        // Find the original card's position, but DON'T move it
        var originalCardNode = NCard.FindOnTable(card);
        if (originalCardNode == null) return;

        var startPos = originalCardNode.GlobalPosition;

        // Create a CLONE for animation
        var cloneCard = NCard.Create(card);
        if (cloneCard == null) return;

        vfx.AddChild(cloneCard);
        cloneCard.GlobalPosition = startPos;
        cloneCard.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);

        var slotIndex = pile.Cards.Count;
        var targetPos = display.GetSlotGlobalPosition(slotIndex);
        var finalSizeHalf = cloneCard.Size * display.Scale / 2f;
        var centeredTarget = targetPos - finalSizeHalf;

        var tween = cloneCard.CreateTween().SetParallel();
        tween.TweenProperty(cloneCard, "global_position", centeredTarget, 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Cubic);

        tween.TweenProperty(cloneCard, "scale", display.Scale * NStasisSlot.CardScale, 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Cubic);

        await display.ToSignal(tween, Tween.SignalName.Finished);

        // Free the CLONE
        cloneCard.QueueFreeSafely();
    }
    */
}
