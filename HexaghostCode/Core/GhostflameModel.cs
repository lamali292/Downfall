using BaseLib.Abstracts;
using Downfall.DownfallCode.Vfx;
using Hexaghost.HexaghostCode.Cards.Uncommon;
using Hexaghost.HexaghostCode.Events;
using Hexaghost.HexaghostCode.Vfx;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
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
    public int IgnitionProgress { get; set; }
    public abstract int IgnitionRequirement { get; }
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

    private int FlameIndex => Array.IndexOf(HexaghostCmd.GetWheel(Owner), this);

    public Player Owner
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

    protected int Repeat(GhostflameRepeatType repeatType)
    {
        return 1+HexaghostHook.ModifyGhostflameRepeatAdditive(Owner.Creature.CombatState!, Owner, repeatType, this);
    }


    public void UpdateVisuals()
    {
        if (!IsActive) return;
        StatusBarHelper.SetStatus(Owner, IgnitionProgress, IgnitionRequirement, FireColor.ToColor());
    }

    protected bool TryProgress(int amount = 1)
    {
        if (IsIgnited || !HexaghostCmd.IsGhostwheelActivated(Owner)) return false;
        IgnitionProgress += amount;
        UpdateVisuals();
        return IgnitionProgress >= IgnitionRequirement;
    }

    public bool Extinguish()
    {
        if (!IsIgnited) return false;
        IsIgnited = false;
        IgnitionProgress = 0;
        UpdateVisuals();
        return true;
    }


    public abstract Task OnIgnite(PlayerChoiceContext ctx);


    public void SetIgniteProgress()
    {
        IgnitionProgress = IgnitionRequirement;
        UpdateVisuals();
    }

    protected async Task Ignite(PlayerChoiceContext ctx)
    {
        await HexaghostCmd.Ignite(ctx, Owner);
    }


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


    private async Task ExecuteWithContext(Func<PlayerChoiceContext, Task> action)
    {
        if (LocalContext.NetId == null) return;
        var ctx = new HookPlayerChoiceContext(
            Owner,
            LocalContext.NetId.Value,
            GameActionType.Combat);
        await ctx.AssignTaskAndWaitForPauseOrCompletion(action(ctx));
    }

    public sealed override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        return ExecuteWithContext(ctx => BeforeCardPlayed(ctx, cardPlay));
    }

    protected virtual Task BeforeCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }

    public sealed override Task AfterEnergySpent(CardModel card, int amount)
    {
        return ExecuteWithContext(ctx => AfterEnergySpent(ctx, card, amount));
    }

    protected virtual Task AfterEnergySpent(PlayerChoiceContext ctx, CardModel card, int amount)
    {
        return Task.CompletedTask;
    }
    
    
    protected bool TryBeginIgnite(string sfx = "event:/sfx/characters/attack_fire")
    {
        if (Owner.Creature.CombatState == null) return false;
        SfxCmd.Play(sfx);
        return true;
    }

    protected async Task RepeatOnTargets(PlayerChoiceContext ctx, int count, GhostflameRepeatType repeatType,
        Func<IReadOnlyList<Creature>, Task> action)
    {
        var hitAll = HexaghostHook.ShouldGhostflameTargetAll(CombatState, this, repeatType, out var matches);
        await HexaghostHook.AfterShouldGhostflameTargetedAll(CombatState, ctx, this, matches);
        for (var i = 0; i < count; i++)
        {
            IReadOnlyList<Creature> targets;
            if (hitAll)
                targets = CombatState.HittableEnemies;
            else
            {
                var target = CombatState.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
                targets = target == null ? [] : [target];
            }
            foreach (var creature in targets)
                SpawnVfx(creature);
            await action(targets);
        }
    }

    protected async Task TriggerOnCardType(PlayerChoiceContext ctx, CardPlay cardPlay, CardType type, Func<CardModel, bool>? cond = null)
    {
        if (!IsActive || cardPlay.Card.Owner != Owner || (cond != null && !cond.Invoke(cardPlay.Card))) return;
        var shouldCount = HexaghostHook.GhostflameConditionOverwrites(CombatState, Owner, this, cardPlay);
        if (cardPlay.Card.Type != type && !shouldCount) return;
        if (!TryProgress()) return;
        await Ignite(ctx);
    }

    public abstract bool AboutToIgnite(CardModel card);
}

public enum GhostflameRepeatType
{
    Soulburn,
    Damage,
    Block
}