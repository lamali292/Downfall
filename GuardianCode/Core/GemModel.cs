using System.Text;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using Guardian.GuardianCode.Cards;
using Guardian.GuardianCode.Cards.Abstract;
using Guardian.GuardianCode.Cards.Basic;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Extensions;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Core;

public abstract class GemModel : CardModifier, ICustomModel
{
    private GemModel _canonicalInstance = null!;
    private DynamicVarSet? _dynamicVars;


    private PowerModel? _power;

    public PowerModel Power
    {
        set
        {
            if (value == _power) throw new Exception($"Power already initialized for {Id}");
            AssertMutable();
            value.AssertMutable();
            _power = value;
        }
    }

    public int SocketIndex => Card is IGemSocketCard socketCard ? socketCard.Gems.IndexOf(this) : -1;

    protected ICombatState CombatState =>
        Card!.CombatState ??
        _power?.CombatState ?? throw new InvalidOperationException($"Gem {Id} has no CombatState!");

    protected Player Player => Card!.Owner;
    public override bool ShouldReceiveCombatHooks => true;

    private string IconName => Id.Entry
        .RemovePrefix()
        .ToLowerInvariant();

    public CardModel? Card => IsCanonical ? null : Owner;


    protected virtual IEnumerable<DynamicVar> CanonicalVars => [];

    protected DynamicVarSet DynamicVars
    {
        get
        {
            if (_dynamicVars != null)
                return _dynamicVars;
            _dynamicVars = new DynamicVarSet(CanonicalVars);
            _dynamicVars.InitializeWithOwner(this);
            return _dynamicVars;
        }
    }

    public GemModel CanonicalInstance
    {
        get => !IsMutable ? this : _canonicalInstance;
        private set
        {
            AssertMutable();
            _canonicalInstance = value;
        }
    }

    public IEnumerable<IHoverTip> HoverTips
    {
        get
        {
            var hoverTips = new List<IHoverTip>();
            hoverTips.Add(ToHoverTip(GetFormattedText()));
            hoverTips.AddRange(ExtraHoverTips);
            return hoverTips;
        }
    }

    public string IconPath => $"{IconName}.png".GemPath();
    private static string EmptyIconPath => "emptysocket.png".GemPath();
    public Texture2D Icon => PreloadManager.Cache.GetAsset<Texture2D>(IconPath);
    public static Texture2D EmptyIcon => PreloadManager.Cache.GetAsset<Texture2D>(EmptyIconPath);
    public abstract Color GemColor { get; }
    public abstract CardRarity Rarity { get; }
    public LocString Title => new("gems", Id.Entry + ".title");
    private LocString Description => new("gems", Id.Entry + ".description");

    public CardModel ToCard => ModelDb.CardPool<GuardianCardPool>().AllCards.OfType<IGemCard>()
        .Where(c => c.CanonicalGemModel.GetType() == GetType()).Cast<CardModel>().First();

    public virtual IEnumerable<IHoverTip> ExtraHoverTips => [];

    public override void ModifyDescription(Creature? target, ref string description)
    {
    }

    public string GetFormattedText(bool cardText = false)
    {
        var stringBuilder = new StringBuilder();
        var isSmart = IsMutable;
        string formatted;
        if (isSmart)
        {
            var locString = Description;
            locString.Add("CardName", Card!.Title);
            AddDumbVariablesToDescription(locString);
            foreach (var dynamicVar in DynamicVars.Values)
            {
                var runGlobalHooks = Card is { CombatState: not null, Pile.Type: PileType.Hand or PileType.Play };
                dynamicVar.UpdateCardPreview(Card, CardPreviewMode.Normal, null, runGlobalHooks);
            }

            DynamicVars.AddTo(locString);
            formatted = locString.GetFormattedText();
        }
        else
        {
            var description = Description;
            AddDumbVariablesToDescription(description);
            DynamicVars.AddTo(description);
            formatted = description.GetFormattedText();
        }

        if (!formatted.Equals(""))
            stringBuilder.Append(formatted);
        return stringBuilder.ToString();
    }

    public GemModel ToMutable()
    {
        AssertCanonical();
        var mutable = (GemModel)MutableClone();
        mutable.CanonicalInstance = this;
        return mutable;
    }

    public GemModel CreateClone()
    {
        AssertMutable();
        return (GemModel)MutableClone();
    }

    public override void AfterClonedOnCard(CardModel card)
    {
        if (card is GuardianCardModel gc)
            OnAdded(gc);
    }

    private HoverTip ToHoverTip(string description)
    {
        return new HoverTip
        {
            CanonicalModel = null,
            ShouldOverrideTextOverflow = false,
            Id = Id.ToString(),
            Title = Title.GetFormattedText(),
            Description = description,
            Icon = Icon,
            IsSmart = true
        };
    }

    private static void AddDumbVariablesToDescription(LocString description)
    {
        description.Add("singleStarIcon", "[img]res://images/packed/sprite_fonts/star_icon.png[/img]");
        description.Add("energyPrefix", EnergyIconHelper.GetPrefix(ModelDb.Card<StrikeGuardian>()));
    }

    protected abstract Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay? cardPlay);
    
    
    public sealed override async Task OnPlay(PlayerChoiceContext ctx, CardPlay? cardPlay)
    {
        var replay = cardPlay?.Card is IGemSocketCard guardianCardModel ? guardianCardModel.GemReplayCount : 1;
        for (var i = 0; i < replay; i++)  await OnPlayInternal(ctx, cardPlay);
        await GuardianHook.AfterGemPlayed(CombatState, ctx, this, cardPlay);
    }

    public virtual int ModifyPlayCount(int originalPlayCount)
    {
        return originalPlayCount;
    }

    internal void ApplyOnAddedEffects(CardModel card)
    {
        if (card is GuardianCardModel gc)
            OnAdded(gc);
    }

    protected virtual void OnAdded(GuardianCardModel card)
    {
    }
}