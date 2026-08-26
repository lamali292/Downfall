using Automaton.AutomatonCode.Vfx;
using Awakened.AwakenedCode.Vfx;
using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Downfall.DownfallCode.Utils.UI;
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
    public override bool CardShouldBeVisible(CardModel card) => false;

    /*
    public override NCard? GetNCard(CardModel card)
    {
        var display = NStashDisplay.GetDisplay(card.Owner);
        return display?.GetNCard(card);
    }*/

    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        return NCustomCombatCardPile.GetPositionFor<NStashPile>();
    }
}