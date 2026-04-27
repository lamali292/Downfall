using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Collector.CollectorCode.Vfx;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Collector.CollectorCode.Piles;

public class CollectorPile() : CustomPile(Collected)
{
    [CustomEnum] public static PileType Collected;

    public override bool CardShouldBeVisible(CardModel card)
    {
        return false;
    }

    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        var container = NCombatRoom.Instance?.Ui.GetNodeOrNull<NCombatPilesContainer>("%CombatPileContainer");
        var btn = container?.GetNodeOrNull<NCollectorPileButton>("CollectorPile");
        if (btn != null)
            return btn.GlobalPosition + new Vector2(25f, 25f); // center of 80x80 button

        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(model.Owner.Creature);
        return creatureNode?.GlobalPosition ?? Vector2.Zero;
    }
}