using Awakened.AwakenedCode.Piles;
using Downfall.DownfallCode.Core;
using Downfall.DownfallCode.Utils.UI;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Vfx;

public partial class NSpellbookButton : NCustomCombatCardPile
{
    protected override PileType Pile => AwakenedPile.Spellbook;
    public override string ScenePath => "res://Awakened/scenes/ui/spellbook_pile.tscn";
    protected override Vector2 HideOffset => new(-160f, 100f);
    protected override Vector2 HoverTipOffset => new(20f, -850f);
    protected override Vector2 ButtonOffsets => new(20f, -360f);
    protected override HoverTip BuildHoverTip()
    {
        var description = new LocString("static_hover_tips", "AWAKENED-SPELLBOOK.description");
        var hasNextSpell = Next != null;
        description.Add("HasSpell", hasNextSpell);
        if (hasNextSpell) description.Add("Spell", Next!.Title);
        return new HoverTip(
            new LocString("static_hover_tips", "AWAKENED-SPELLBOOK.title"),
            description
        );
    }
    
    private static readonly PlayerField<NSpellbookButton> Instance = new(() => null);

    public override void Initialize(Player player)
    {
        base.Initialize(player);
        Instance[player] = this;
    }

    public static void RevealFor(Player player)
    {
        var btn = Instance[player];
        if (btn != null && IsInstanceValid(btn)) btn.Reveal();
    }
    
    protected override bool StartHidden(Player player)
    {
        return player.Character is not Core.Awakened;
    }

    private CardModel? Next => (_pile as AwakenedPile)?.NextSpell;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        Next == null ?  [] : [HoverTipFactory.FromCard(Next), ..Next.HoverTips];

    protected override LocString BuildEmptyPileMessage()
    {
        return new LocString("combat_messages", "OPEN_EMPTY_SPELLBOOK");
    }

  
}