// Downfall/Code/Cards/Automaton/FunctionCard.cs

using System.Runtime.InteropServices;
using System.Text;
using Automaton.AutomatonCode.Interfaces;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Extensions;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Pooling;

namespace Automaton.AutomatonCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public sealed class FunctionCard() : AutomatonCardModel(1, CardType.Skill,
    CardRarity.Token, TargetType.AnyEnemy)
{
    
    
    public IEnumerable<DynamicVarSet> GetDynamicVars()
    {
        return SourceCards.Select(t => t.DynamicVars
        );
    }
    
    private ImageTexture? _cachedPortrait;
    private CardType _cardType;
    private IReadOnlyList<CardModel> _lastPortraitSource = [];
    public IReadOnlyList<CardModel> SourceCards = [];
    private TargetType _targetType;
    private CardRarity _cardRarity;

    public override string CustomPortraitPath => "function_card.tres".CardImageAtlasPath<Core.Automaton>();
    //public override string CustomPortraitPath => "function_card.png".CardImagePath<Character.Automaton>();

    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;
    public override int MaxUpgradeLevel => 0;

    public override bool HasBuiltInOverlay => false;


    public override CardRarity Rarity => _cardRarity;
    public override CardType Type => _cardType;
    public override TargetType TargetType => _targetType;

    public void SetSourceCards(IReadOnlyList<CardModel> sourceCards)
    {
        SourceCards = sourceCards;
    }
    


    public string GetDynamicTitle()
    {
        if (SourceCards.Count == 0)
            return new LocString("cards", Id.Entry + ".title").GetFormattedText();

        var sb = new StringBuilder();

        for (var i = 0; i < SourceCards.Count; i++)
        {
            var card = SourceCards[i];
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
        var i = 0;
        var lines = new List<string>();
        foreach (var card in SourceCards)
        {
            if (card is IEncodable encodable)
            {
                var text = encodable.GetEncodeLocString(new EncodeContext(true, i))?.GetFormattedText();
                if (text == null) continue;
                lines.Add(text);
            }
            else
            {
                var loc = card.Description;
                card.DynamicVars.AddTo(loc);
                var upgradeDisplay = !card.IsUpgraded ? UpgradeDisplay.Normal : UpgradeDisplay.Upgraded;

                loc.Add(new IfUpgradedVar(upgradeDisplay));
                loc.Add("OnTable", false);
                loc.Add("InCombat", 0);
                loc.Add("TargetType", card.TargetType.ToString());
                loc.Add("GainsBlock", card.GainsBlock);
                var prefix = EnergyIconHelper.GetPrefix(card);
                loc.Add("energyPrefix", prefix);
                loc.Add("singleStarIcon", "[img]res://images/packed/sprite_fonts/star_icon.png[/img]");
                foreach (var variable3 in loc.Variables)
                {
                  if (variable3.Value is EnergyVar energyVar)
                    energyVar.ColorPrefix = prefix;
                }
                lines.Add(loc.GetFormattedText());
            }
            i++;
        }
        description.Add("effects", lines.Count > 0
            ? string.Join("\n", lines)
            : "");
    }


    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        for (var i = 0; i < SourceCards.Count; i++)
        {
            var card = SourceCards[i];
            if (card is IEncodable encodable)
                await encodable.PlayEncodableEffect(ctx, cardPlay, new EncodeContext(true, i));
            else
            {
                await DownfallCardCmd.OnPlay.Invoke(card, ctx, cardPlay);
            }
            
        }
            
        if (Type == CardType.Power)
        {
            var power = await PowerCmd.Apply<FullReleasePower>(ctx,
                Owner.Creature, 1, Owner.Creature, this);
            power?.SetSourceCards(SourceCards);
        }
    }

    public ImageTexture? GetCompositePortrait()
    {
        if (_cachedPortrait != null && SourceCards.SequenceEqual(_lastPortraitSource))
            return _cachedPortrait;

        var images = SourceCards
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

        _lastPortraitSource = SourceCards.ToList();
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
    
    public void SetCardRarity(CardRarity cardRarity)
    {
        _cardRarity = cardRarity;
    }
}


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

[HarmonyPatch(typeof(NCard), "Reload")]
public static class NCardPortraitPatch
{
    private static void Postfix(NCard __instance)
    {
        if (__instance.Model is not FunctionCard fc) return;

        var portraitRect = __instance.GetNode<TextureRect>("%Portrait");
        var ancientPortraitRect = __instance.GetNode<TextureRect>("%AncientPortrait");

        foreach (var node in new[] { portraitRect, ancientPortraitRect })
        {
            if (node == null) continue;
            foreach (var child in node.GetChildren()
                         .Where(c => c.Name.ToString().StartsWith("_composite_")))
                child.QueueFree();
        }

        var textures = fc.SourceCards
            .Select(c => c.Portrait)
            
            .ToList();

        if (textures.Count == 0) return;

        portraitRect.Texture = null;
        ancientPortraitRect.Texture = null;

        for (var i = 0; i < textures.Count; i++)
        {
            var src = textures[i];
            var w = src.GetWidth();
            var h = src.GetHeight();
            var sliceW = w / textures.Count;

            var atlas = new AtlasTexture { Atlas = src, Region = new Rect2(i * sliceW, 0, sliceW, h) };

            foreach (var node in new[] { portraitRect, ancientPortraitRect })
            {
                node.AddChild(new TextureRect
                {
                    Name = $"_composite_{i}",
                    Texture = atlas,
                    AnchorLeft = (float)i / textures.Count,
                    AnchorRight = (float)(i + 1) / textures.Count,
                    AnchorTop = 0,
                    AnchorBottom = 1,
                    OffsetLeft = 0, OffsetRight = 0, OffsetTop = 0, OffsetBottom = 0,
                    ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                    StretchMode = TextureRect.StretchModeEnum.Scale,
                    MouseFilter = Control.MouseFilterEnum.Ignore
                });
            }
        }
    }
}


[HarmonyPatch(typeof(NCard), nameof(NCard.Create))]
public static class NCardCreatePatch
{
    private static bool Prefix(CardModel card, ModelVisibility visibility, ref NCard? __result)
    {
        if (card is not FunctionCard) return true;
        var scene = ResourceLoader.Load<PackedScene>(NCard._scenePath);
        var ncard = scene.Instantiate<NCard>();
        ncard.Model = card;
        ncard.Visibility = visibility;
        __result = ncard;
        return false;
    }
}

[HarmonyPatch(typeof(NodePool), nameof(NodePool.Free), typeof(IPoolable))]
public static class NodePoolFreePatch
{
    private static bool Prefix(IPoolable poolable)
    {
        if (poolable is not NCard { Model: FunctionCard } ncard) return true;
        ncard.QueueFree();
        return false;
    }
}
