using Automaton.AutomatonCode.Vfx;
using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

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
        var display = NSequenceDisplay.GetDisplay(model.Owner);
        return display?.GlobalPosition ?? Vector2.Zero;
    }
}