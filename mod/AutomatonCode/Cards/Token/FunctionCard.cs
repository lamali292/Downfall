// Downfall/Code/Cards/Automaton/FunctionCard.cs

using Automaton.AutomatonCode.Functions;
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
    private IReadOnlyList<FunctionContribution> _contributions = [];
    public IReadOnlyList<CardModel> SourceCards => _sourceCards;

    /// The contributions that currently apply: those whose var is above 0, or that always apply.
    private IEnumerable<FunctionContribution> Active => _contributions.Where(c => c.IsActive(this));

    protected override IEnumerable<DynamicVar> CanonicalVars => FunctionAssembler.CanonicalVars;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        Active.SelectMany(c => c.HoverTips?.Invoke(this) ?? []);

    public override int MaxUpgradeLevel => 0;
    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;
    public override bool GainsBlock => Active.Any(c => c.GainsBlock);

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
        foreach (var contribution in Active.Where(c => c.Play != null))
        {
            await contribution.Play!(this, ctx, cardPlay.Target, cardPlay);
            if (contribution.EndsSequence)
                break;
        }
    }

    private CardType CalcType()
    {
        var types = Active.Select(c => c.Type).OfType<CardType>().ToList();
        if (types.Contains(CardType.Power)) return CardType.Power;
        if (types.Contains(CardType.Attack)) return CardType.Attack;
        if (types.Contains(CardType.Skill)) return CardType.Skill;
        return CardType.None;
    }

    private TargetType CalcTarget()
    {
        var active = Active.ToList();
        if (active.Any(c => c.ForcesSelfTarget)) return TargetType.Self;

        var targets = active.Select(c => c.Target).OfType<TargetType>().ToList();
        if (targets.Contains(TargetType.AnyEnemy)) return TargetType.AnyEnemy;
        if (targets.Contains(TargetType.AllEnemies)) return TargetType.AllEnemies;
        if (targets.Contains(TargetType.Self)) return TargetType.Self;
        return TargetType.None;
    }

    /// <summary>Called by <see cref="FunctionAssembler" /> once the source cards' values are merged into this Function's vars.</summary>
    internal void SetAssembly(IReadOnlyList<CardModel> sourceCards, IReadOnlyList<FunctionContribution> contributions,
        string title)
    {
        _sourceCards = sourceCards.ToList();
        _contributions = contributions;
        _dynamicTitle = title;
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        // Only the contributions flagged for card text; the rest are listed in NFunctionDisplay.
        description.Add("effects", string.Join("\n", Lines(c => c.ShownOnCard)));
    }

    /// <summary>Formatted text of every applying contribution that belongs to <paramref name="keyword" />, one line each.</summary>
    public IEnumerable<string> GetLines(CardKeyword keyword)
    {
        return Lines(c => c.Keyword == keyword);
    }

    private IEnumerable<string> Lines(Func<FunctionContribution, bool> filter)
    {
        return Active.Where(filter)
            .Select(c => c.Line?.Invoke(this))
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l => l!);
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