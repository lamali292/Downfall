using BaseLib.Abstracts;
using Downfall.Code.Core.Hexaghost.Ghostflames;
using Downfall.Code.Vfx.Hexaghost;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Core.Hexaghost;

// GhostflameModel.cs
public abstract class GhostflameModel : AbstractModel, ICustomModel
{
    private GhostflameModel _canonicalInstance;
    private Player? _owner;
    public override bool ShouldReceiveCombatHooks => true;

    protected bool IsActive => HexaghostCmd.GetCurrentFlame(Owner) == this;
    public bool IsIgnited { get; set; }
    private int IgnitionProgress { get; set; }
    protected abstract int IgnitionRequirement { get; }
    public LocString Title => new("ghostflames", Id.Entry + ".title");
    public LocString Description => new("ghostflames", Id.Entry + ".description");
    public abstract NFire.FireColor FireColor { get; }
    protected ICombatState CombatState => Owner.Creature.CombatState!;
    public HoverTip HoverTip
    {
        get
        {
            var tip = new HoverTip(Title, Description);
            tip.SetCanonicalModel(CanonicalInstance);
            return tip;
        }
    }
    
    protected Player Owner
    {
        get
        {
            AssertMutable();
            return _owner!;
        }
        private set
        {
            AssertMutable();
            _owner = _owner == null || _owner == value
                ? value
                : throw new InvalidOperationException($"Cannot move ghostflame {Id.Entry} from one owner to another");
        }
    }

    private GhostflameModel CanonicalInstance
    {
        get => !IsMutable ? this : _canonicalInstance;
        set
        {
            AssertMutable();
            _canonicalInstance = value;
        }
    }

    protected bool TryProgress()
    {
        if (IsIgnited) return false;
        IgnitionProgress++;
        return IgnitionProgress >= IgnitionRequirement;
    }

    public void Extinguish()
    {
        IsIgnited = false;
        IgnitionProgress = 0;
    }


    public abstract Task OnIgnite(PlayerChoiceContext ctx);


    protected async Task Ignite(PlayerChoiceContext ctx)
    {
        if (IsIgnited) return;
        IsIgnited = true;
        HexaghostVisualsBridge.Refresh(Owner);
        await OnIgnite(ctx);
    }

    public GhostflameModel ToMutable(Player player)
    {
        AssertCanonical();
        var mutable = (GhostflameModel)MutableClone();
        mutable.CanonicalInstance = this;
        mutable.Owner = player;
        return mutable;
    }
}