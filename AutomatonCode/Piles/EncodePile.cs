using Automaton.AutomatonCode.Displays;
using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Automaton.AutomatonCode.Piles;

public class EncodePile() : CustomPile(FunctionSequence)
{
    [CustomEnum] public static PileType FunctionSequence;

    public override bool CardShouldBeVisible(CardModel card)
    {
        return true;
    }


    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        var display = AutomatonDisplay.GetDisplay(model.Owner);
        if (display != null)
        {
            var slotIndex = FunctionSequence.GetPile(model.Owner).Cards.IndexOf(model);
            return display.GetSlotGlobalPosition(slotIndex < 0 ? 0 : slotIndex);
        }

        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(model.Owner.Creature);
        return creatureNode?.GlobalPosition ?? Vector2.Zero;
    }
}