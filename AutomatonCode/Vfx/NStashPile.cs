using Automaton.AutomatonCode.Piles;
using Godot;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Vfx;

public partial class NStashPile : NCreatureFollowingCardPile
{
    protected override PileType Pile => StashPile.Stash;
    public override string ScenePath => "res://Automaton/scenes/ui/stash_pile.tscn";
    protected override Vector2 HideOffset => new(-160f, 100f);
    protected override Vector2 HoverTipOffset => new(30f, -850f);
    protected override Vector2 ButtonOffsets => new(115f, -360f);
    protected override Vector2 FollowOffset => new(-150f, -250f);

    protected override bool StartHidden(Player player)
        => !LocalContext.IsMe(player) || player.Character is not Core.Automaton;

    protected override HoverTip BuildHoverTip()
        => new(new LocString("static_hover_tips", "AUTOMATON-STASH.title"),
            new LocString("static_hover_tips", "AUTOMATON-STASH.description"));

    protected override LocString BuildEmptyPileMessage()
        => new("combat_messages", "OPEN_EMPTY_STASH");
    
    protected override List<CardModel> GetCards()
    {
        var card = _pile?.Cards.FirstOrDefault();
        
        return card == null ? [] : [card];
    }
    
    public static void RevealFor(Player player)
    {
        if (!LocalContext.IsMe(player)) return;   // only the local player's pile reveals
        var btn = GetPileNode<NStashPile>();
        btn?.Reveal();
    }
}