using Automaton.AutomatonCode.Vfx;
using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Automaton.AutomatonCode.Piles;

public class StashPile() : CustomPile(Stash)
{
    [CustomEnum] public static PileType Stash;

    // no custom transition, no GetNCard
    public override bool NeedsCustomTransitionVisual => false;

    // cards are NOT visible in the pile itself
    public override bool CardShouldBeVisible(CardModel card)
    {
        return false;
    }

    /*
    public override LocString Name => new("card_selection", "AUTOMATON-STASH_PILE");

    public override string IconPath =>
        ImageHelper.GetImagePath($"atlases/power_atlas.sprites/strength_power.tres");
    */

    public override NCard? GetNCard(CardModel card)
    {
        return null;
    }

    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        var display = NStashDisplay.GetDisplay(model.Owner);
        return display?.GlobalPosition ?? Vector2.Zero;
    }
}