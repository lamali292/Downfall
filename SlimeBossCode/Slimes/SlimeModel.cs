using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Compatibility;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using SlimeBoss.SlimeBossCode.DynamicVars;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public abstract class SlimeModel : CustomMonsterModel, ICustomAbstractModel
{
    private DynamicVarSet? _dynamicVars;
    public override int MinInitialHp => Really.bigNumber;
    public override int MaxInitialHp => Really.bigNumber;
    
    public override string CustomVisualPath
    {
        get
        {
            var path = $"combat/{Id.Entry.RemovePrefix().ToLowerInvariant()}.tscn".SlimeScenePath();
            return ResourceLoader.Exists(path) ? path : "combat/guerilla_slime.tscn".SlimeScenePath();
        }
    }

    /// <summary>
    /// Name of the Spine skin this slime wants on its skeleton, or null to leave whatever skin the
    /// scene already has active. Override this instead of SetupSkins for the common case.
    /// </summary>
    protected virtual string? SkinName => null;

    /// <summary>
    /// Applies SkinName if it names a skin that actually exists on this skeleton - which can miss
    /// when CustomVisualPath had to fall back to guerilla_slime.tscn because this slime has no
    /// scene/skin of its own yet. Leaves the scene's current skin alone otherwise, rather than
    /// clearing it. Override this instead of SkinName if a slime needs more than a skin swap.
    /// </summary>
    public override void SetupSkins(MegaSprite spine, MegaSkeleton skeleton)
    {
        if (SkinName != null)
        {
            var skin = skeleton.GetData().FindSkin(SkinName);
            if (skin != null) skeleton.SetSkin(skin);
        }

        skeleton.SetSlotsToSetupPose();
    }

    public override bool HasDeathSfx => false;
    public Creature PetOwner => Creature.PetOwner?.Creature ?? throw new ArgumentNullException(nameof(PetOwner));
    public Player Player => PetOwner.Player ?? throw new ArgumentNullException(nameof(Player));
    protected virtual LocString Description => L10NMonsterLookup(Id.Entry + ".description");


    public sealed override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        return SetupAnimationState(controller, "idle_loop", hitName: "hurt", attackName: "attack");
    }
 
    private LocString SmartDescription
    {
        get
        {
            var description = Description;
            UpdatePreviewValues();
            DynamicVars.AddTo(description);
            return description;
        }
    }

    public HoverTip SlimeTip => new(Title, SmartDescription);

    public virtual IEnumerable<IHoverTip> ExtraTips => [];

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

    protected override void DeepCloneFields()
    {
        _dynamicVars = DynamicVars.Clone(this);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var initialState = new MoveState("NOTHING_MOVE", _ => Task.CompletedTask);
        initialState.FollowUpState = initialState;
        return new MonsterMoveStateMachine([initialState], initialState);
    }

    /// <summary>
    /// Runs this slime's attack/effect. <paramref name="forcedTarget"/> overrides the slime's normal
    /// targeting (random/all opponents, "last attacked enemy", etc.) with a specific enemy - used by
    /// effects like "Command ALL Slimes to attack the targeted enemy" (e.g. Slime Brawl).
    /// </summary>
    public abstract Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null);


    /// <summary>The var whose value the corner counter shows: this slime's damage, or its secondary value.</summary>
    public virtual DynamicVar? CounterVar => DynamicVars.Values.FirstOrDefault(v =>
        v is DamageVar or CalculatedDamageVar or SlimeSecondaryVar);

    /// <summary>Icon on the corner counter; override for slimes whose main effect isn't damage.</summary>
    public virtual string CounterIconPath => "res://images/packed/intents/attack/intent_attack_1.png";

    /// <summary>What the next Command will do, with Potency and other modifiers applied. Null if nothing to show.</summary>
    public decimal? CounterValue
    {
        get
        {
            var counterVar = CounterVar;
            if (counterVar == null) return null;
            UpdatePreviewValues();
            return counterVar.PreviewValue;
        }
    }

    /// <summary>
    /// When non-null, the counter shows this icon alone (no number) instead of its usual value - for
    /// phases where the next Command does nothing, e.g. a sleeping slime.
    /// </summary>
    public virtual string? CounterStatusIconPath => null;

    /// <summary>How many times the counter's effect repeats per Command; shown as "damage x hits" when above 1.</summary>
    public virtual int CounterHitCount => 1;

    /// <summary>The unmodified value <see cref="CounterValue"/> is compared against to colour the counter.</summary>
    public decimal CounterBaseValue => CounterVar?.BaseValue ?? 0m;

    protected virtual void UpdatePreviewValues()
    {
        if (IsCanonical) return;
        if (Creature is not { IsAlive: true }) return;

        foreach (var dynamicVar in DynamicVars.Values)
            switch (dynamicVar)
            {
                case CustomModelCalculatedDamageVar calc:
                    calc.PreviewValue = CompatibilityHook.ModifyDamage(CombatState.RunState, CombatState, null, Creature,
                        calc.CalculateCustom(null), calc.Props, null, null, ModifyDamageHookType.All, CardPreviewMode.Normal, out _);
                    break;
                case DamageVar dmg:
                    dmg.PreviewValue = CompatibilityHook.ModifyDamage(
                        CombatState.RunState,
                        CombatState,
                        null,
                        Creature,
                        dmg.BaseValue,
                        dmg.Props,
                        null,
                        null,
                        ModifyDamageHookType.All,
                        CardPreviewMode.Normal,
                        out _);
                    break;
                case SlimeSecondaryVar snd:
                    snd.PreviewValue = snd.ToPreviewValue(
                        SlimeBossHook.ModifySlimeEffects(CombatState, snd.IntValue, out _, this), this);
                    break;
            }
    }
}