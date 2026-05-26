using BaseLib.Extensions;
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
    public IHoverTip HoverTip
    {
        get
        {
            var title = new LocString("champ_stances", $"{GetType().GetPrefix()}{Id.Entry}.title");
            var description = new LocString("champ_stances", $"{GetType().GetPrefix()}{Id.Entry}.description");
            DynamicVars.AddTo(description);
            return new HoverTip(title, description);
        }
    }
    protected override void DeepCloneFields()
    {
        _dynamicVars = DynamicVars.Clone(this);
    }

    
    protected virtual IEnumerable<DynamicVar> CanonicalVars => [];

    private Player? _player;

    public int Charges;
    public abstract bool HasFinisher { get; }
    public virtual string? ChargeIconPath => null;
    public Player Owner => _player ?? throw new InvalidOperationException("Not a mutable instance");

    public ICombatState CombatState => Owner.Creature.CombatState ??
                                          throw new InvalidOperationException("Combat state not initialized");

    public ChampStanceModel ToMutable(Player player)
    {
        var mutable = (ChampStanceModel)MutableClone();
        mutable._player = player;
        return mutable;
    }

    public void ResetCharges()
    {
        Charges = 3;
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

    public virtual Task Finisher(PlayerChoiceContext ctx)
    {
        return Task.CompletedTask;
    }
}