using Awakened.AwakenedCode.Interfaces;
using Awakened.AwakenedCode.Piles;
using Downfall.DownfallCode.Core;
using Downfall.DownfallCode.Utils.UI;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Vfx;

public partial class NSpellbookButton : NCustomCombatCardPile
{
    private static readonly PlayerField<bool> Revealed = new(() => false);
    protected override PileType Pile => AwakenedPile.Spellbook;
    public override string ScenePath => "res://Awakened/scenes/ui/spellbook_pile.tscn";
    protected override Vector2 HideOffset => new(-160f, 100f);
    protected override Vector2 HoverTipOffset => new(30f, -850f);
    protected override Vector2 ButtonOffsets => new(20f, -360f);

    private CardModel? Next => (_pile as AwakenedPile)?.NextSpell;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        Next == null ? [] : [HoverTipFactory.FromCard(Next), ..Next.HoverTips];

    public override void Initialize(Player player)
    {
        base.Initialize(player);
        if (Revealed[player]) Visible = true;
        RefreshSpellIcon();
    }

    public static void RevealFor(Player player)
    {
        Revealed[player] = true;
        var btn = GetPileNode<NSpellbookButton>();
        if (btn == null) return;
        btn.Reveal();
        btn.RefreshSpellIcon();
    }


    public void RefreshSpellIcon()
    {
        var slot = GetNodeOrNull<TextureRect>("Icon");
        if (slot == null) return;

        if (Next is ISpell spell && ResourceLoader.Exists(spell.SpellIconPath))
            slot.Texture = PreloadManager.Cache.GetTexture2D(spell.SpellIconPath);
    }

    protected override bool StartHidden(Player player)
    {
        return player.Character is not Core.Awakened;
    }

    protected override HoverTip BuildHoverTip()
    {
        var description = new LocString("static_hover_tips", "AWAKENED-SPELLBOOK.description");
        var hasNextSpell = Next != null;
        description.Add("HasSpell", hasNextSpell);
        if (hasNextSpell) description.Add("Spell", Next!.Title);
        return new HoverTip(
            new LocString("static_hover_tips", "AWAKENED-SPELLBOOK.title"),
            description);
    }

    protected override LocString BuildEmptyPileMessage()
    {
        return new LocString("combat_messages", "OPEN_EMPTY_SPELLBOOK");
    }
}