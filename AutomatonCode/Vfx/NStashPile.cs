using Automaton.AutomatonCode.Piles;
using Downfall.DownfallCode.Utils.UI;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace Automaton.AutomatonCode.Vfx;

public partial class NStashPile  : NCustomCombatCardPile
{
    protected override PileType Pile => StashPile.Stash;
    public override string ScenePath => "res://Automaton/scenes/ui/stash_pile.tscn";
    protected override Vector2 HideOffset => new(-160f, 100f);
    protected override Vector2 HoverTipOffset => new(30f, -850f);
    protected override Vector2 ButtonOffsets => new(20f, -360f);
   
    protected override HoverTip BuildHoverTip()
    {
        var description = new LocString("static_hover_tips", "AUTOMATON-STASH.description");
        return new HoverTip(
            new LocString("static_hover_tips", "AUTOMATON-STASH.title"),
            description);
    }

    protected override LocString BuildEmptyPileMessage()
        => new("combat_messages", "OPEN_EMPTY_STASH");
}