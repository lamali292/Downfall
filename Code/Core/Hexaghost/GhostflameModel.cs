using BaseLib.Abstracts;
using Downfall.Code.Commands;
using Downfall.Code.Core.Hexaghost.Ghostflames;
using Downfall.Code.Vfx.Hexaghost;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Core.Hexaghost;

// GhostflameModel.cs
public abstract class GhostflameModel : AbstractModel, ICustomModel
{
    public override bool ShouldReceiveCombatHooks => true;

    protected bool IsActive => HexaghostCmd.GetCurrentFlame(Owner) == this;
    public bool IsIgnited { get; set; }
    private int IgnitionProgress { get; set; }
    protected abstract int IgnitionRequirement { get; }
    public LocString Title => new("hexaghost", Id.Entry + ".title");
    public LocString Description => new("hexaghost", Id.Entry + ".description");

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
    public abstract NFire.FireColor FireColor { get; }
    private Player? _owner;
    protected ICombatState CombatState => Owner.Creature.CombatState!;

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
            _owner = _owner == null || _owner == value ? value : throw new InvalidOperationException($"Cannot move ghostflame {Id.Entry} from one owner to another");
        }
    }
    private GhostflameModel _canonicalInstance;
    private GhostflameModel CanonicalInstance
    {
        get => !IsMutable ? this : _canonicalInstance;
        set
        {
            AssertMutable();
            _canonicalInstance = value;
        }
    }
    
    public GhostflameModel ToMutable(Player player)
    {
        AssertCanonical();
        var mutable = (GhostflameModel) MutableClone();
        mutable.CanonicalInstance = this;
        mutable.Owner = player;
        return mutable;
    }
}