// Downfall/Code/Cards/Automaton/FunctionCard.cs

using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Interfaces;
using Downfall.DownfallCode.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Automaton.AutomatonCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public sealed class FunctionCard() : CustomCardModel(1, CardType.Skill,
    CardRarity.Token, TargetType.AnyEnemy), ICustomPortrait
{
    private IReadOnlyList<CardModel> _cachedSourceCards = [];
    private ImageTexture? _cachedTexture;
    private string _dynamicTitle = string.Empty;

    private IReadOnlyList<CardModel> _sourceCards = [];
    protected override IEnumerable<DynamicVar> CanonicalVars => Encodable.All.Select(e => e.FunctionDynamicVar);

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        Encodable.All.SelectMany(e => e.DynamicVar(this).BaseValue > 0 ? e.HoverTips(this) : []);

    public override int MaxUpgradeLevel => 0;
    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;
    public override bool GainsBlock => DynamicVars.Block.BaseValue > 0;

    public override TargetType TargetType => CalcTarget();
    public override CardType Type => CalcType();

    public override string CustomPortraitPath => "function_card.tres".CardImageAtlasPath<Core.Automaton>();
    public override string Title => _dynamicTitle.Equals(string.Empty) ? base.Title : _dynamicTitle;

    public Texture2D? GetPortraitTexture()
    {
        return GetTexture();
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        foreach (var encodable in Encodable.All)
            if (encodable.DynamicVar(this).BaseValue > 0)
            {
                await encodable.OnPlay(this, ctx, cardPlay.Target, cardPlay);
                if (encodable is PowerEncode)
                    break;
            }
    }

    private CardType CalcType()
    {
        var targetTypes = Encodable.All.Where(e => e.DynamicVar(this).BaseValue > 0).Select(e => e.Type).Distinct()
            .ToList();
        if (targetTypes.Contains(CardType.Power)) return CardType.Power;
        if (targetTypes.Contains(CardType.Attack)) return CardType.Attack;
        if (targetTypes.Contains(CardType.Skill)) return CardType.Skill;
        return CardType.None;
    }

    private TargetType CalcTarget()
    {
        var encoded = Encodable.All.Where(e => e.DynamicVar(this).BaseValue > 0).ToList();
        if (encoded.Any(e => e is PowerEncode)) return TargetType.Self;

        var targetTypes = encoded.Select(e => e.Target).Distinct()
            .ToList();
        if (targetTypes.Contains(TargetType.AnyEnemy)) return TargetType.AnyEnemy;
        if (targetTypes.Contains(TargetType.AllEnemies)) return TargetType.AllEnemies;
        if (targetTypes.Contains(TargetType.Self)) return TargetType.Self;
        return TargetType.None;
    }

    public void SetSourceCards(IReadOnlyList<CardModel> sourceCards)
    {
        _sourceCards = sourceCards.ToList();
        foreach (var canonicalVar in CanonicalVars) canonicalVar.BaseValue = 0;

        if (sourceCards.Count <= 0) return;
        _dynamicTitle = GetDynamicTitle(_sourceCards);

        var max = AutomatonCmd.GetMax(_sourceCards[0].Owner);

        var i = 1;
        foreach (var sourceCard in _sourceCards)
        {
            var pos = i == 1 ? FunctionPosition.Start : i == max ? FunctionPosition.End : FunctionPosition.Middle;
            if (sourceCard is not IEncodable encodable) continue;
            encodable.ApplyEncode(this, pos);
            foreach (var encodableEncoding in encodable.Encodings) encodableEncoding.ApplyEncode(this, sourceCard);

            i++;
        }
    }


    protected override void AddExtraArgsToDescription(LocString description)
    {
        var lines = (from encodable in Encodable.All
            where encodable.DynamicVar(this).BaseValue > 0
            select encodable.GetDescription(this).GetFormattedText()).ToList();
        description.Add("effects", string.Join("\n", lines.Where(l => !string.IsNullOrWhiteSpace(l))));
    }


    private string GetDynamicTitle(IReadOnlyList<CardModel> sourceCards)
    {
        if (sourceCards.Count == 0)
            return new LocString("cards", Id.Entry + ".title").GetFormattedText();

        if (sourceCards is [Constructor, Separator, Terminator] or [Constructor, Separator, Separator, Terminator])
        {
            var perfection = new LocString("encode", "AUTOMATON-PERFECTION.functionName").GetFormattedText();
            return perfection;
        }

        var prefix = Encode(0, ".functionPrefix", card => card.Title.ToLowerInvariant());
        var name = Encode(1, ".functionName", card => card.Title);
        var end3 = Encode(2, ".functionEnd", card => card.Title[0].ToString());
        var end4 = Encode(3, ".functionEnd", card => card.Title[0].ToString());
        var parenthesesLoc = new LocString("encode", "AUTOMATON-FUNCTION.functionParentheses");
        var parentheses = parenthesesLoc.Exists() ? parenthesesLoc.GetFormattedText() : "()";

        var functionName = new LocString("encode", "AUTOMATON-FUNCTION.title");

        functionName.Add("prefix", prefix);
        functionName.Add("name", name);
        functionName.Add("end3", end3);
        functionName.Add("end4", end4);
        functionName.Add("parentheses", parentheses);
        return functionName.GetFormattedText();

        string Encode(int index, string suffix, Func<CardModel, string>? fallback = null)
        {
            if (sourceCards.Count <= index)
                return "";

            var loc = new LocString("encode", sourceCards[index].Id.Entry + suffix);
            return loc.Exists() ? loc.GetFormattedText() : fallback?.Invoke(sourceCards[index]) ?? "";
        }
    }


    private ImageTexture? GetTexture()
    {
        if (_cachedTexture != null &&
            _cachedSourceCards.SequenceEqual(_sourceCards))
            return _cachedTexture;

        var textures = _sourceCards
            .Select(c => ResourceLoader.Load<Texture2D>(c.PortraitPath))
            .ToList();

        var composite = PortraitCompositor.SliceHorizontally(textures);
        if (composite == null) return null;

        _cachedTexture = composite;
        _cachedSourceCards = _sourceCards;
        return _cachedTexture;
    }
}

public enum FunctionPosition
{
    Start,
    Middle,
    End
}