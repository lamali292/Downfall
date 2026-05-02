using System.Text;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using Guardian.GuardianCode.Cards;
using Guardian.GuardianCode.Cards.Abstract;
using Guardian.GuardianCode.Cards.Basic;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Core;

public abstract class GemModel : AbstractModel, ICustomModel
{
    private GemModel _canonicalInstance = null!;

    private GuardianCardModel? _card;
    public int SocketIndex => _card?.Gems.IndexOf(this) ?? -1;
    private PowerModel? _power;
    private DynamicVarSet? _dynamicVars;

    protected ICombatState CombatState =>
        Card.CombatState ??  _power?.CombatState  ?? throw new InvalidOperationException($"Gem {Id} has no CombatState!");

    protected Player Player => Card.Owner;
    public override bool ShouldReceiveCombatHooks => true;

    private string IconName => Id.Entry
        .RemovePrefix()
        .ToLowerInvariant();

    
    public PowerModel Power
    {
        get
        {
            AssertMutable();
            return _power ?? throw new InvalidOperationException($"Gem {Id} has no Power attached to it!");
        }
        set
        {
            if (value == _power) throw new Exception($"Power already initialized for {Id}");
            AssertMutable();
            value.AssertMutable();
            _power = value;
        }
    }
    
    
    public GuardianCardModel Card
    {
        get
        {
            AssertMutable();
            return _card ?? throw new InvalidOperationException($"Gem {Id} has no Card attached to it!");
        }
        set
        {
            if (value == _card) throw new Exception($"Card already initialized for {Id}");
            var previousCard = _card;
            AssertMutable();
            value.AssertMutable();
            _card = value;
            OnAdded(_card);
            if (previousCard != null) OnRemoved(previousCard);
        }
    }
    

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

            var isSmart = HasSmartDescription && IsMutable;
            var a = GetFormattedText();
            hoverTips.Add(ToHoverTip(a, isSmart));
            hoverTips.AddRange(ExtraHoverTips);
            return hoverTips;
        }
    }


    private LocString SmartDescription =>
        !HasSmartDescription ? Description : new LocString("gems", SmartDescriptionLocKey);

    private bool HasSmartDescription => LocString.Exists("gems", SmartDescriptionLocKey);


    public string IconPath => $"{IconName}.png".GemPath();
    private static string EmptyIconPath => "emptysocket.png".GemPath();
    public Texture2D Icon => PreloadManager.Cache.GetAsset<Texture2D>(IconPath);
    public static Texture2D EmptyIcon => PreloadManager.Cache.GetAsset<Texture2D>(EmptyIconPath);
    public abstract Color GemColor { get; }
    public abstract CardRarity Rarity { get; }
    public LocString Title => new("gems", Id.Entry + ".title");
    private LocString Description => new("gems", Id.Entry + ".description");
    private bool HasDescriptionExtra => LocString.Exists("gems", Id.Entry + ".descriptionExtra");
    private LocString DescriptionExtra => new("gems", Id.Entry + ".descriptionExtra");
    private string SmartDescriptionLocKey => Id.Entry + ".smartDescription";


    
    public CardModel ToCard => ModelDb.CardPool<GuardianCardPool>().AllCards.OfType<IGemCard>()
        .Where(c => c.CanonicalGemModel.GetType() == GetType()).Cast<CardModel>().First();

    public virtual IEnumerable<IHoverTip> ExtraHoverTips => [];

    public string GetFormattedText(bool cardText = false)
    {
        var stringBuilder = new StringBuilder();
        var isSmart = HasSmartDescription && IsMutable;
        string formatted;
        if (isSmart)
        {
            var locString = SmartDescription;
            locString.Add("CardName", Card.Title);
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
            formatted = description.GetFormattedText();
         
        }

        var isEmpty = formatted.Equals("");
        if (!isEmpty)
            stringBuilder.Append(formatted);
        if (cardText || !HasDescriptionExtra) return stringBuilder.ToString();
        if (!isEmpty)
        {
            stringBuilder.Append("\n");
        }
        stringBuilder.Append(DescriptionExtra.GetFormattedText());

        return stringBuilder.ToString();
    }


    public GemModel ToMutable()
    {
        AssertCanonical();
        var mutable = (GemModel)MutableClone();
        mutable.CanonicalInstance = this;
        return mutable;
    }


    private HoverTip ToHoverTip(string description, bool isSmart)
    {
        var a = new HoverTip
        {
            CanonicalModel = null,
            ShouldOverrideTextOverflow = false,
            Id = Id.ToString(),
            Title = Title.GetFormattedText(),
            Description = description,
            Icon = Icon,
            IsSmart = isSmart
        };
        return a;
    }

    private void AddDumbVariablesToDescription(LocString description)
    {
        description.Add("singleStarIcon", "[img]res://images/packed/sprite_fonts/star_icon.png[/img]");
        description.Add("energyPrefix", EnergyIconHelper.GetPrefix(ModelDb.Card<StrikeGuardian>()));
    }

    public async Task OnPlayWrapper(PlayerChoiceContext ctx, CardPlay? cardPlay)
    {
        await OnPlay(ctx, cardPlay);
        await GuardianHook.AfterGemPlayed(CombatState, ctx, this,  cardPlay);
    }


    protected abstract Task OnPlay(PlayerChoiceContext ctx, CardPlay? cardPlay);


    public virtual int ModifyPlayCount(int originalPlayCount)
    {
        return originalPlayCount;
    }
    
    public GemModel CreateClone()
    {
        AssertMutable();
        var a = (GemModel)MutableClone();
        return a;
    }

    protected virtual void OnAdded(CardModel card)
    {
    }

    protected virtual void OnRemoved(CardModel card)
    {
    }
}