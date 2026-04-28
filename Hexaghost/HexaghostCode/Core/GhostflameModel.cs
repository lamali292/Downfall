using System.Numerics;
using BaseLib.Abstracts;
using Hexaghost.HexaghostCode.Events;
using Hexaghost.HexaghostCode.Vfx;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Vector2 = Godot.Vector2;

namespace Hexaghost.HexaghostCode.Core;

// GhostflameModel.cs
public abstract class GhostflameModel : AbstractModel, ICustomModel
{
    private GhostflameModel? _canonicalInstance;
    private Player? _owner;
    public override bool ShouldReceiveCombatHooks => true;
    public abstract AbstractIntent Intent { get; }
    public bool IsActive => HexaghostCmd.GetCurrentFlame(Owner) == this;
    public bool IsIgnited { get; set; }
    private int IgnitionProgress { get; set; }
    protected abstract int IgnitionRequirement { get; }
    public LocString Title => new("ghostflames", Id.Entry + ".title");
    public LocString Description => new("ghostflames", Id.Entry + ".description");
    public abstract FireColor FireColor { get; }
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
    
    protected int Intensity => HexaghostHook.ModifyGhostflameEffectAdditive(Owner.Creature.CombatState!, Owner, this);
    protected int Repeat(GhostflameRepeatType repeatType) => HexaghostHook.ModifyGhostflameRepeatAdditive(Owner.Creature.CombatState!, Owner, repeatType, this);

    private int FlameIndex => Array.IndexOf(HexaghostCmd.GetWheel(Owner), this);
    
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
        get => !IsMutable ? this : _canonicalInstance!;
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

    public bool Extinguish()
    {
        if (!IsIgnited)  return false;
        IsIgnited = false;
        IgnitionProgress = 0;
        return true;
    }


    public abstract Task OnIgnite(PlayerChoiceContext ctx);


    protected async Task Ignite(PlayerChoiceContext ctx)
        => await HexaghostCmd.Ignite(ctx, Owner);

    public GhostflameModel ToMutable(Player player)
    {
        AssertCanonical();
        var mutable = (GhostflameModel)MutableClone();
        mutable.CanonicalInstance = this;
        mutable.Owner = player;
        return mutable;
    }

    protected void SpawnVfx(Creature target)
    {
        var visuals = HexaghostVisualsBridge.GetVisuals(Owner);
        var from = visuals?.GetFlameWorldPosition(FlameIndex) + 30 * Vector2.Up
                   ?? Owner.Creature.GetCreatureNode()?.VfxSpawnPosition
                   ?? Vector2.Zero;
        var to = target.GetCreatureNode()?.VfxSpawnPosition ?? Vector2.Zero;
        var fireball = NFireballEffect.Create(from, to, FireColor.ToColor());
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(fireball);
    }
}

public enum GhostflameRepeatType
{
    Soulburn,
    Damage,
    Block
}
