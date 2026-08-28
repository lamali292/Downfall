using Automaton.AutomatonCode.Vfx;
using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Downfall.DownfallCode.Utils.UI;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Piles;

public class StashPile() : CustomPile(Stash)
{
    [CustomEnum] public static PileType Stash;

    public override bool CardShouldBeVisible(CardModel card) => true;
    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        return NCustomCombatCardPile.GetPositionFor<NStashPile>();
    }
}