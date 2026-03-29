using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Downfall.Code.Piles;

public class AutomatonPile : CustomPile
{
    [CustomEnum] public static PileType FunctionSequence;

    // No-parameter constructor — required by BaseLib's reflection
    public AutomatonPile() : base(FunctionSequence)
    {
    }

    public override bool CardShouldBeVisible(CardModel card)
    {
        return false;
    }

    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(model.Owner.Creature);
        if (creatureNode == null) return Vector2.Zero;

        var index = Cards.IndexOf(model);
        var totalWidth = Cards.Count * (size.X + 8f);
        var startX = creatureNode.GlobalPosition.X - totalWidth / 2f + index * (size.X + 8f);
        var y = creatureNode.GlobalPosition.Y - creatureNode.Size.Y / 2f - size.Y - 16f;

        return new Vector2(startX, y);
    }
}