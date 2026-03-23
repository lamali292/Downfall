// Downfall/Code/Cards/Automaton/FunctionCard.cs

using System.Text;
using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Powers.Automaton;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Automaton.Token;


#pragma warning disable STS001
[Pool(typeof(TokenCardPool))]
public sealed class FunctionAttackCard() : FunctionCard(CardType.Attack, TargetType.AnyEnemy);

[Pool(typeof(TokenCardPool))]
public sealed class FunctionSkillCard() : FunctionCard(CardType.Skill, TargetType.Self);

[Pool(typeof(TokenCardPool))]
public sealed class FunctionPowerCard() : FunctionCard(CardType.Power, TargetType.None);
#pragma warning restore STS001

public abstract class FunctionCard(CardType type, TargetType targetType) : AutomatonCardModel(1, type,
    CardRarity.Token, targetType)

{
    private ImageTexture? _cachedPortrait;
    private IReadOnlyList<AutomatonCardModel> _lastPortraitSource = [];
    private IReadOnlyList<AutomatonCardModel> _sourceCards = [];

    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;
    public override int MaxUpgradeLevel => 0;

    public void SetSourceCards(IReadOnlyList<AutomatonCardModel> sourceCards)
    {
        _sourceCards = sourceCards;
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
            var power = await PowerCmd.Apply<FullReleasePower>(
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
        if (_cachedPortrait != null && Equals(_sourceCards, _lastPortraitSource))
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

        var result = Image.CreateEmpty(w, h, false, Image.Format.Rgba8);

        for (var i = 0; i < images.Count; i++)
        {
            var src = images[i];
            if (src.GetWidth() != w || src.GetHeight() != h)
                src.Resize(w, h);

            // last slice takes any remaining pixels to avoid gaps
            var width = i == images.Count - 1 ? w - i * sliceW : sliceW;
            result.BlitRect(src, new Rect2I(i * sliceW, 0, width, h), new Vector2I(i * sliceW, 0));
        }

        _lastPortraitSource = _sourceCards;
        _cachedPortrait = ImageTexture.CreateFromImage(result);
        return _cachedPortrait;
    }

    public override bool HasBuiltInOverlay => true;
}

[HarmonyPatch(typeof(CardModel), "get_OverlayPath")]
public static class OverlayPathPatch
{
    public static bool Prefix(CardModel __instance, ref string __result)
    {
        if (__instance is not FunctionCard) return true;
        
        __result = "res://Downfall/scenes/cards/overlays/function_card.tscn";
        return false;
    }
}