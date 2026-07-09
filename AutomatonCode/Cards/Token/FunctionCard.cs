// Downfall/Code/Cards/Automaton/FunctionCard.cs

using System.Text;
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
    private string _dynamicTitle = string.Empty;
    protected override IEnumerable<DynamicVar> CanonicalVars => Encodable.All.Select(e => e.FunctionDynamicVar);

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        Encodable.All.SelectMany(e => e.DynamicVar(this).BaseValue > 0 ? e.HoverTips(this) : []);

    public override int MaxUpgradeLevel => 0;
    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;
    public override bool GainsBlock => DynamicVars.Block.BaseValue > 0;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        foreach (var encodable in Encodable.All)
            if (encodable.DynamicVar(this).BaseValue > 0)
                await encodable.OnPlay(this, ctx, cardPlay.Target, cardPlay);
    }

    public override TargetType TargetType => CalcTarget();
    public override CardType Type => CalcType();

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
        var targetTypes = Encodable.All.Where(e => e.DynamicVar(this).BaseValue > 0).Select(e => e.Target).Distinct()
            .ToList();
        if (targetTypes.Contains(TargetType.AnyEnemy)) return TargetType.AnyEnemy;
        if (targetTypes.Contains(TargetType.AllEnemies)) return TargetType.AllEnemies;
        if (targetTypes.Contains(TargetType.Self)) return TargetType.Self;
        return TargetType.None;
    }

    private IReadOnlyList<CardModel> _sourceCards = [];
    private ImageTexture? _cachedTexture;
    private IReadOnlyList<CardModel> _cachedSourceCards = [];

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

        var sb = new StringBuilder();

        for (var i = 0; i < sourceCards.Count; i++)
        {
            var card = sourceCards[i];
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
                    // Don't use id for this, lol
                    sb.Append(card.Title[0]);
                    break;
            }
        }

        sb.Append("()");
        return sb.ToString();
    }


    private ImageTexture? GetTexture()
    {
        if (_cachedTexture != null &&
            _cachedSourceCards.SequenceEqual(_sourceCards))
        {
            return _cachedTexture;
        }

        var textures = _sourceCards
            .Select(c => ResourceLoader.Load<Texture2D>(c.PortraitPath))
            .ToList();

        var composite = PortraitCompositor.SliceHorizontally(textures);
        if (composite == null) return null;

        _cachedTexture = composite;
        _cachedSourceCards = _sourceCards;
        return _cachedTexture;
    }

    public override string CustomPortraitPath => "function_card.tres".CardImageAtlasPath<Core.Automaton>();
    public override string Title => _dynamicTitle.Equals(string.Empty) ? base.Title : _dynamicTitle;
    public Texture2D? GetPortraitTexture() => GetTexture();
}

public enum FunctionPosition
{
    Start,
    Middle,
    End
}