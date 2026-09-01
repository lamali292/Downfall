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
    protected override Vector2 HideOffset => new(-0, 0);
    protected override Vector2 HoverTipOffset => new(0, 0);
    protected override Vector2 ButtonOffsets => new(0, 0);
    protected override Vector2 FollowOffset => new(150f, -250f);
    protected override float BobSpeed => 1.8f;       
    protected override bool StartHidden(Player player)
        => !LocalContext.IsMe(player) || player.Character is not Core.Automaton;

    protected override HoverTip BuildHoverTip()
        => new(new LocString("static_hover_tips", "AUTOMATON-ENCODE_PILE.title"),
            new LocString("static_hover_tips", "AUTOMATON-ENCODE_PILE.description"));

    protected override LocString BuildEmptyPileMessage()
        => new("combat_messages", "OPEN_EMPTY_ENCODE");
    
    
    private CardModel? _previewModel;
    private readonly List<CardModel> _previewSource = new();
    
    protected override List<CardModel> GetCards()
    {
        var list = _pile?.Cards;
        if (list == null || list.Count == 0)
        {
            _previewModel = null;
            _previewSource.Clear();
            return [];
        }

        // Rebuild only when the source cards changed.
        if (_previewModel != null && list.SequenceEqual(_previewSource))
            return _previewModel == null ? [] : [_previewModel];
        _previewSource.Clear();
        _previewSource.AddRange(list);
        _previewModel = CreatePreviewModel(_previewSource);

        return _previewModel == null ? [] : [_previewModel];
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