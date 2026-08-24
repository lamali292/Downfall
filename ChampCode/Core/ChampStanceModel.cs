using BaseLib.Extensions;
using Champ.ChampCode.Events;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Core;

public abstract class ChampStanceModel : AbstractModel
{
    private DynamicVarSet? _dynamicVars;

    private Player? _player;

    public virtual int MaxCharges => 3;
    public int Charges;

    public DynamicVarSet DynamicVars
    {
        get
        {
            if (_dynamicVars != null) return _dynamicVars;
            _dynamicVars = new DynamicVarSet(CanonicalVars);
            _dynamicVars.InitializeWithOwner(this);
            return _dynamicVars;
        }
    }

    public IEnumerable<IHoverTip> HoverTips => [HoverTip, ..ExtraHoverTips];

    protected virtual IEnumerable<IHoverTip> ExtraHoverTips => [];
    
    public IHoverTip HoverTip
    {
        get
        {
            var title = new LocString("champ_stances", $"{GetType().GetPrefix()}{Id.Entry}.title");
            var description = new LocString("champ_stances", $"{GetType().GetPrefix()}{Id.Entry}.description");
            DynamicVars.AddTo(description);
            if(IsMutable)
            {
                description.Add("Infinite",   _player is { Creature.CombatState: not null } && ChampHook.IgnoreChargeCap(_player.Creature.CombatState, _player));
                description.Add("Charges", Charges);
            } else {
                description.Add("Infinite",   false);
                description.Add("Charges", MaxCharges);
            }
            return new HoverTip(title, description);
        }
    }


    protected virtual IEnumerable<DynamicVar> CanonicalVars => [];
    public abstract bool HasFinisher { get; }
    public virtual string? ChargeIconPathOver => null;
    public virtual string? ChargeIconPathProgress => null;
    public virtual string? ChargeIconPathUnder => null;
    
    private Lazy<Texture2D?>? _lazyProgress;
    private Lazy<Texture2D?>? _lazyOver;
    private Lazy<Texture2D?>? _lazyUnder;
    public Texture2D? ChargeTextureProgress => (_lazyProgress ??= CreateLazyTexture(ChargeIconPathProgress)).Value;
    public Texture2D? ChargeTextureOver => (_lazyOver ??= CreateLazyTexture(ChargeIconPathOver)).Value;
    public Texture2D? ChargeTextureUnder => (_lazyUnder ??= CreateLazyTexture(ChargeIconPathUnder)).Value;
    
    private static Lazy<Texture2D?> CreateLazyTexture(string? path)
    {
        return new Lazy<Texture2D?>(() =>
            !string.IsNullOrEmpty(path) ? ResourceLoader.Load<Texture2D>(path) : null);
    }
    
    public virtual Color? LabelOutlineColor => null;
    
    public Player Owner => _player ?? throw new InvalidOperationException("Not a mutable instance");

    public ICombatState CombatState => Owner.Creature.CombatState ??
                                       throw new InvalidOperationException("Combat state not initialized");

    protected override void DeepCloneFields()
    {
        _dynamicVars = DynamicVars.Clone(this);
    }

    public ChampStanceModel ToMutable(Player player)
    {
        var mutable = (ChampStanceModel)MutableClone();
        mutable._player = player;
        return mutable;
    }

    public void ResetCharges()
    {
        Charges = MaxCharges;
        ChampModel.RefreshDisplay(Owner);
    }

    public Task OnEnter(PlayerChoiceContext ctx)
    {
        ResetCharges();
        return Task.CompletedTask;
    }

    public Task OnExit(PlayerChoiceContext ctx)
    {
        Charges = 0;
        return Task.CompletedTask;
    }

    public virtual Task SkillBonus(PlayerChoiceContext ctx)
    {
        return Task.CompletedTask;
    }

    public virtual Task Finisher(PlayerChoiceContext ctx, bool affectsAllPlayers)
    {
        return Task.CompletedTask;
    }
}