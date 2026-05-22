// Downfall/Code/Cards/Automaton/FunctionCard.cs

using System.Text;
using Automaton.AutomatonCode.Interfaces;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Extensions;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Automaton.AutomatonCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public sealed class FunctionCard() : AutomatonCardModel(1, CardType.Skill,
    CardRarity.Token, TargetType.AnyEnemy)
{
    private ImageTexture? _cachedPortrait;
    private CardType _cardType;
    private IReadOnlyList<CardModel> _lastPortraitSource = [];
    private IReadOnlyList<CardModel> _sourceCards = [];
    private TargetType _targetType;

    public override string CustomPortraitPath => "function_card.tres".CardImageAtlasPath<Core.Automaton>();
    //public override string CustomPortraitPath => "function_card.png".CardImagePath<Character.Automaton>();

    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;
    public override int MaxUpgradeLevel => 0;

    public override bool HasBuiltInOverlay => false;


    public override CardType Type => _cardType;

    public override TargetType TargetType => _targetType;

    public void SetSourceCards(IReadOnlyList<CardModel> sourceCards)
    {
        _sourceCards = sourceCards;
    }

    public IEnumerable<DynamicVarSet> GetDynamicVars()
    {
        return _sourceCards.Select(t => t.DynamicVars
        );
    }

    public string GetDynamicTitle()
    {
        if (_sourceCards.Count == 0)
            return new LocString("cards", Id.Entry + ".title").GetFormattedText();

        var sb = new StringBuilder();

        for (var i = 0; i < _sourceCards.Count; i++)
        {
            var card = _sourceCards[i];
            switch (i)
            {
                case 0:
                    var prefix = new LocString("encode", card.Id.Entry + ".functionPrefix");
                    sb.Append(prefix.Exists() ? prefix.GetFormattedText() : "");
                    break;
                case 1:
                    var name = new LocString("encode", card.Id.Entry + ".functionName");
                    sb.Append(name.Exists() ? name.GetFormattedText() : "");
                    break;
                case 2:
                case 3:
                    sb.Append(card.Title[0]);
                    break;
            }
        }

        sb.Append("()");
        return sb.ToString();
    }

    // Build description from source card effects
    protected override void AddExtraArgsToDescription(LocString description)
    {
        var lines = _sourceCards
            .Select((c, i) => (c as IEncodable)?.GetEncodeLocString(
                new EncodeContext(true, i))
            ).Where(loc => loc != null)
            .Select(loc => loc!.GetFormattedText())
            .ToList();

        description.Add("effects", lines.Count > 0
            ? string.Join("\n", lines)
            : "");
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (Type == CardType.Power)
        {
            var power = await PowerCmd.Apply<FullReleasePower>(ctx,
                Owner.Creature, 1, Owner.Creature, this);
            power?.SetSourceCards(_sourceCards);
        }
        else
        {
            for (var i = 0; i < _sourceCards.Count; i++)
                if (_sourceCards[i] is IEncodable encodable)
                    await encodable.PlayEncodableEffect(ctx, cardPlay, new EncodeContext(true, i));
        }
    }

    public ImageTexture? GetCompositePortrait()
    {
        if (_cachedPortrait != null && _sourceCards.SequenceEqual(_lastPortraitSource))
            return _cachedPortrait;

        var images = _sourceCards
            .Select(c => ResourceLoader.Load<Texture2D>(c.PortraitPath)?.GetImage())
            .Where(img => img != null)
            .Cast<Image>()
            .ToList();

        if (images.Count == 0) return null;

        var w = images[0].GetWidth();
        var h = images[0].GetHeight();
        var sliceW = w / images.Count;

        // Use Rgba8 as the standard
        var result = Image.CreateEmpty(w, h, false, Image.Format.Rgba8);

        for (var i = 0; i < images.Count; i++)
        {
            var src = images[i];
            if (src.GetFormat() != Image.Format.Rgba8) src.Convert(Image.Format.Rgba8);
            if (src.IsCompressed()) src.Decompress();

            if (src.GetWidth() != w || src.GetHeight() != h)
                src.Resize(w, h);

            var width = i == images.Count - 1 ? w - i * sliceW : sliceW;
            result.BlitRect(src, new Rect2I(i * sliceW, 0, width, h), new Vector2I(i * sliceW, 0));
        }

        _lastPortraitSource = _sourceCards.ToList();
        _cachedPortrait = ImageTexture.CreateFromImage(result);
        return _cachedPortrait;
    }

    public void SetCardType(CardType cardType)
    {
        _cardType = cardType;
    }

    public void SetTargetType(TargetType targetType)
    {
        _targetType = targetType;
    }
}

/*
[HarmonyPatch(typeof(CardModel), "get_OverlayPath")]
public static class OverlayPathPatch
{
    public static bool Prefix(CardModel __instance, ref string __result)
    {
        if (__instance is not FunctionCard) return true;

        __result = "res://Automaton/scenes/ui/function_card.tscn";
        return false;
    }
}*/

[HarmonyPatch(typeof(CardModel), "get_Title")]
public static class FunctionCardTitlePatch
{
    private static bool Prefix(CardModel __instance, ref string __result)
    {
        if (__instance is not FunctionCard fc) return true;

        var txt = fc.GetDynamicTitle();
        if (!__instance.IsUpgraded)
            __result = txt;
        else if (__instance.MaxUpgradeLevel <= 1)
            __result = txt + "+";
        else
            __result = $"{txt}+{__instance.CurrentUpgradeLevel}";
        return false;
    }
}

[HarmonyPatch(typeof(NCard))]
[HarmonyPatch("Reload")]
public static class NCardPortraitPatch
{
    private static void Postfix(NCard __instance)
    {
        if (__instance.Model is not FunctionCard fc) return;

        var composite = fc.GetCompositePortrait();
        if (composite == null) return;

        var portrait = __instance.GetNode<TextureRect>("%Portrait");
        if (portrait != null)
            portrait.Texture = composite;
    }
}