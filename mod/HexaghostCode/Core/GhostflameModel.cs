using System.Text;
using BaseLib.Abstracts;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Vfx;
using Hexaghost.HexaghostCode.DynamicVars;
using Hexaghost.HexaghostCode.Events;
using Hexaghost.HexaghostCode.Interfaces;
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
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;
using Vector2 = Godot.Vector2;

namespace Hexaghost.HexaghostCode.Core;

// GhostflameModel.cs
public abstract class GhostflameModel : AbstractModel, ICustomModel, ICustomAbstractModel
{
    Creature ICustomAbstractModel.Creature => Owner.Creature;
    Player ICustomAbstractModel.Player => Owner;


    private DynamicVarSet? _dynamicVars;
    public override bool ShouldReceiveCombatHooks => true;
    public abstract AbstractIntent Intent { get; }
    protected bool IsActive => HexaghostCmd.GetCurrentFlame(Owner) == this;
    public bool IsIgnited { get; set; }
    protected int IgnitionProgress { get; set; }
    protected abstract int IgnitionRequirement { get; }
    public LocString Title => new("ghostflames", Id.Entry + ".title");
    private LocString Description => new("ghostflames", Id.Entry + ".description");
    public abstract FireColor FireColor { get; }
    private ICombatState CombatState => Owner.Creature.CombatState!;
    public virtual bool IsOffclass => false;

    public DynamicVarSet DynamicVars
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

    protected virtual IEnumerable<DynamicVar> CanonicalVars => [];


    private HoverTip HoverTip => ToHoverTip(GetFormattedText());


    protected virtual IEnumerable<IHoverTip> ExtraHoverTips => [];


    public IEnumerable<IHoverTip> HoverTips
    {
        get
        {
            var hoverTips = new List<IHoverTip> { HoverTip };
            hoverTips.AddRange(ExtraHoverTips);
            return hoverTips.Distinct();
        }
    }

    protected IRunState RunState => Owner.RunState;
    
    private int FlameIndex => Array.IndexOf(HexaghostCmd.GetWheel(Owner), this);

    public Player Owner
    {
        get
        {
            AssertMutable();
            return field!;
        }
        private set
        {
            AssertMutable();
            field = field == null || field == value
                ? value
                : throw new InvalidOperationException($"Cannot move ghostflame {Id.Entry} from one owner to another");
        }
    }

    private GhostflameModel CanonicalInstance
    {
        get => !IsMutable ? this : field!;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    private string GetFormattedText()
    {
        var stringBuilder = new StringBuilder();
        var locString = Description;
        var prefix = Owner.Character.CardPool.EnergyColorName;
        locString.Add("energyPrefix", prefix);
        locString.Add("TargetsAll", TargetsAllEnemies());
        UpdateDynamicVarPreview();
        DynamicVars.AddTo(locString);
        var formatted = locString.GetFormattedText();
        if (!formatted.Equals(""))
            stringBuilder.Append(formatted);
        return stringBuilder.ToString();
    }

    protected override void DeepCloneFields()
    {
        _dynamicVars = DynamicVars.Clone(this);
    }

    private HoverTip ToHoverTip(string description)
    {
        return new HoverTip(Title, description).WithIdAndSmart(Id.ToString(), true);
    }

    /// <summary>Whether a hook (Inferno Potion, ...) currently makes this flame's damage or Soulburn hit ALL enemies.</summary>
    private bool TargetsAllEnemies()
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null) return false;
        return (DynamicVars.Values.OfType<GhostflameDamageVar>().Any() &&
                HexaghostHook.ShouldGhostflameTargetAll(combatState, this, GhostflameRepeatType.Damage, out _)) ||
               (DynamicVars.Values.OfType<GhostflameSoulburnVar>().Any() &&
                HexaghostHook.ShouldGhostflameTargetAll(combatState, this, GhostflameRepeatType.Soulburn, out _));
    }

    private void UpdateDynamicVarPreview()
    {
        foreach (var dynamicVar in DynamicVars.Values.OfType<GhostflameVar>().ToList())
            dynamicVar.UpdateGhostflamePreview(this, true);
    }

    protected int Repeat(GhostflameRepeatType repeatType)
    {
        return 1 + HexaghostHook.ModifyGhostflameRepeatAdditive(Owner.Creature.CombatState!, Owner, repeatType, this);
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

    /// <summary>Ignites this flame - not the current one, which may differ when the card that triggered it moved the wheel.</summary>
    protected async Task Ignite(PlayerChoiceContext ctx)
    {
        await HexaghostCmd.IgniteAt(ctx, Owner, FlameIndex);
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

    // A primed flame already shows as ignited while its card is played; the ignition itself still comes after the card.
    protected virtual Task BeforeCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (IsActive && IsPrimedBy(cardPlay.Card))
            HexaghostCmd.Refresh(Owner, cardPlay.Card);
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
        if (TestMode.IsOff) SfxCmd.Play(sfx);
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
            {
                targets = CombatState.HittableEnemies;
            }
            else
            {
                var target = RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
                targets = target == null ? [] : [target];
            }

            foreach (var creature in targets)
                SpawnVfx(creature);
            await action(targets);
        }
    }

    /// <summary>
    /// Ignition progress that playing <paramref name="card" /> (spending <paramref name="energySpent" />) gives this
    /// flame (0 = none). The single rule behind both the real after-play trigger and the <see cref="IsPrimedBy" />
    /// preview, so what a card reads can never differ from what then actually happens.
    /// </summary>
    protected abstract int ProgressFrom(CardModel card, int energySpent);

    /// <summary>
    /// <see cref="ProgressFrom" /> for flames that progress by 1 per played card of <paramref name="type" />, with the
    /// shared filters (current flame, own card, <see cref="IDoesNotTriggerGhostflame" />, condition overwrites).
    /// </summary>
    protected int ProgressOnCardType(CardModel card, CardType type, Func<CardModel, bool>? cond = null)
    {
        if (card.Owner != Owner || card is IDoesNotTriggerGhostflame ||
            (cond != null && !cond.Invoke(card))) return 0;
        var shouldCount = HexaghostHook.GhostflameConditionOverwrites(CombatState, Owner, this, card);
        return card.Type == type || shouldCount ? 1 : 0;
    }

    /// <summary>Flames ignite after the card resolved, so the card's own effect never sees its own ignition.</summary>
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        // Only the flame the card was played on reacts, even if the card moved the wheel since.
        if (HexaghostCmd.GetFlameBeforePlay(cardPlay.Card) != this) return;
        var amount = ProgressFrom(cardPlay.Card, cardPlay.Resources.EnergySpent);
        if (amount <= 0) return;
        // The card ignited this flame by hand and its play would have ignited it anyway: the prime still fires.
        if (IsIgnited ? HexaghostCmd.WasPrimedAndIgnitedByPlay(cardPlay.Card, this) : TryProgress(amount))
            await Ignite(ctx);
    }

    /// <summary>
    /// Whether playing <paramref name="card" /> will ignite this flame once it resolved ("primed"). Cards use it
    /// (via <see cref="HexaghostCmd.GetIgnitedCount(Player, CardModel)" />) to count the flame their own play ignites
    /// without the ignition effect itself - damage, Soulburn, ... - firing before the card's own effect.
    /// </summary>
    public bool IsPrimedBy(CardModel card)
    {
        return IsActive && WouldIgniteFrom(card);
    }

    /// <summary>Whether playing <paramref name="card" /> on this flame would ignite it, whichever flame is current.</summary>
    public bool WouldIgniteFrom(CardModel card)
    {
        if (IsIgnited || !HexaghostCmd.IsGhostwheelActivated(Owner)) return false;
        var progress = ProgressFrom(card, card.EnergyCost.GetAmountToSpend());
        return progress > 0 && IgnitionProgress + progress >= IgnitionRequirement;
    }
}

public enum GhostflameRepeatType
{
    Soulburn,
    Damage,
    Block,
    Intensity
}