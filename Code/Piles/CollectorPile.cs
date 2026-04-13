using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Downfall.Code.Piles;

public class CollectorPile (): CustomPile(Collected)
{
    [CustomEnum] public static PileType Collected;
    public override bool CardShouldBeVisible(CardModel card)
    {
        return false;
    }
    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(model.Owner.Creature);
        return creatureNode?.GlobalPosition ?? Vector2.Zero;
    }
}