using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using Guardian.GuardianCode.Displays;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Guardian.GuardianCode.Piles;

public class GuardianPile : CustomPile
{
    [CustomEnum] public static PileType Stasis;

    // No-parameter constructor — required by BaseLib's reflection
    public GuardianPile() : base(Stasis)
    {
    }

    public override bool CardShouldBeVisible(CardModel card)
    {
        return true;
    }

    public override NCard? GetNCard(CardModel card)
    {
        return GuardianDisplay.GetNCard(card);
    }

    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(model.Owner.Creature);
        return GuardianDisplay.GetPosition(model) ?? creatureNode?.GetTopOfHitbox() ?? Vector2.Zero;
    }
}