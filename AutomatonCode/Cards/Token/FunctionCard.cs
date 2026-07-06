// Downfall/Code/Cards/Automaton/FunctionCard.cs

using System.Text;
using Automaton.AutomatonCode.Cards.Rare;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Interfaces;
using Automaton.AutomatonCode.Powers;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
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
    public static readonly SpireField<CardModel, bool> IsInFunction = new(() => false);


    public static readonly AsyncLocal<FunctionCard?> CurrentlyExecuting = new();
    
    private CardRarity _cardRarity;
    private CardType _cardType;
    private TargetType _targetType;

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
        var modifiers = sourceCards.SelectMany(CardModifier.Modifiers).OfType<EncodeModifier>()
            .Select(e => e.MutableClone()).OfType<EncodeModifier>();
        foreach (var encodeModifier in modifiers)
        {
            CardModifier.AddModifier(this, encodeModifier);
        }

        ApplyFunctionCardType(sourceCards);
    }

    private void ApplyFunctionCardType(IEnumerable<CardModel> snapshot)
    {
        var list = snapshot.ToList();

        if (list.Any(c => c is { TargetType: TargetType.AnyEnemy }))
            SetTargetType(TargetType.AnyEnemy);
        else if (list.Any(c => c is { TargetType: TargetType.AllEnemies }))
            SetTargetType(TargetType.AllEnemies);
        else
            SetTargetType(TargetType.Self);

        if (list.Any(c => c is FullRelease))
            SetCardType(CardType.Power);
        else if (list.Any(c => c is { Type: CardType.Attack }))
            SetCardType(CardType.Attack);
        else
            SetCardType(CardType.Skill);

        if (list.Any(c => c.Rarity == CardRarity.Ancient))
            SetCardRarity(CardRarity.Ancient);
        else if (list.Any(c => c.Rarity == CardRarity.Rare))
            SetCardRarity(CardRarity.Rare);
        else if (list.Any(c => c.Rarity == CardRarity.Uncommon))
            SetCardRarity(CardRarity.Uncommon);
        else
            SetCardRarity(CardRarity.Common);
    }

    public string GetDynamicTitle()
    {
        var encoding = Encoding;
        if (encoding.Count == 0)
            return new LocString("cards", Id.Entry + ".title").GetFormattedText();

        var sb = new StringBuilder();

        for (var i = 0; i < encoding.Count; i++)
        {
            var card = encoding[i];
            switch (i)
            {
                case 0:
                    var prefix = new LocString("encode", card.Identifier + ".functionPrefix");
                    sb.Append(prefix.Exists() ? prefix.GetFormattedText() : "");
                    break;
                case 1:
                    var name = new LocString("encode", card.Identifier + ".functionName");
                    sb.Append(name.Exists() ? name.GetFormattedText() : "");
                    break;
                case 2:
                case 3:
                    // Don't use id for this, lol
                    sb.Append(card.Identifier.RemovePrefix()[0]);
                    break;
            }
        }

        sb.Append("()");
        return sb.ToString();
    }

    private List<EncodeModifier> Encoding => CardModifier.Modifiers(this).OfType<EncodeModifier>().ToList();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var previous = CurrentlyExecuting.Value;
        CurrentlyExecuting.Value = this;
        try
        {
            if (Type == CardType.Power)
            {
                var power = await PowerCmd.Apply<FullReleasePower>(ctx,
                    Owner.Creature, 1, Owner.Creature, this);
                power?.SetSourceCards(Encoding);
            }
        }
        finally
        {
            CurrentlyExecuting.Value = previous;
        }
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

[HarmonyPatch(typeof(CardModel), "get_CombatState")]
public static class CardModelCombatStatePatch
{
    private static void Postfix(CardModel __instance, ref ICombatState? __result)
    {
        if (__result != null) return;
        if (FunctionCard.CurrentlyExecuting.Value == null) return;
        if (__instance is FunctionCard) return;
        __result = FunctionCard.CurrentlyExecuting.Value.Owner.Creature.CombatState;
    }
}

[HarmonyPatch(typeof(NCard), "Reload")]
public static class NCardPortraitPatch
{
    private static void Postfix(NCard __instance)
    {
        if (__instance.Model is not FunctionCard fc) return;
/*
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
        */
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