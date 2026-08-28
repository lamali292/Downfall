using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Events;
using Automaton.AutomatonCode.Piles;
using Godot;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Vfx;

public partial class NEncodePile : NCreatureFollowingCardPile
{
    protected override PileType Pile => EncodePile.FunctionSequence;
    public override string ScenePath => "res://Automaton/scenes/ui/encode_pile.tscn";
    protected override Vector2 HideOffset => new(-160f, 100f);
    protected override Vector2 HoverTipOffset => new(30f, -850f);
    protected override Vector2 ButtonOffsets => new(115f, -360f);
    protected override Vector2 FollowOffset => new(150f, -250f);
    protected override float BobSpeed => 1.8f;       
    protected override bool StartHidden(Player player)
        => !LocalContext.IsMe(player) || player.Character is not Core.Automaton;

    protected override HoverTip BuildHoverTip()
        => new(new LocString("static_hover_tips", "AUTOMATON-ENCODE.title"),
            new LocString("static_hover_tips", "AUTOMATON-ENCODE.description"));

    protected override LocString BuildEmptyPileMessage()
        => new("combat_messages", "OPEN_EMPTY_ENCODE");
    
    protected override List<CardModel> GetCards()
    {
        var list = _pile?.Cards.ToList();
        if (list == null) return [];
        var function = CreatePreviewModel(list);
        return function == null? [] : [function];
    }

    private static CardModel? CreatePreviewModel(IReadOnlyList<CardModel> slotCards)
    {
        if (ModelDb.Card<FunctionCard>().ToMutable() is not FunctionCard model) return null;
        if (slotCards.Count <= 0) return null;
        var player = slotCards[0].Owner;
        model.SetSourceCards(slotCards);
        model.Owner = player;
        return AutomatonHook.ModifyCompiledFunction(player.Creature.CombatState!, model,
            player, out _);

    }
    
    public static void RevealFor(Player player)
    {
        if (!LocalContext.IsMe(player)) return;   // only the local player's pile reveals
        var btn = GetPileNode<NEncodePile>();
        btn?.Reveal();
    }

}