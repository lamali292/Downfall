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
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SlimeBoss.SlimeBossCode.DynamicVars;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Vfx;

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

    public int SlimeAmount
    {
        get;
        set
        {
            var old = field;
            field = value;
            SlimeAmountChanged(old, value);
        }
    } = 1;

    protected virtual void SlimeAmountChanged(int oldValue, int newValue)
    {
        SlimeBossMainFile.Logger.Info($"Slimecount: {oldValue} -> {newValue}");
        if (NCombatRoom.Instance?.GetCreatureNode(Creature)?.Visuals is NMultiSlimeVisuals visuals)
            visuals.SetSlimeCount(newValue);
    }

    public virtual SlimeType SlimeType => SlimeType.Single;
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


    protected virtual void UpdatePreviewValues()
    {
        if (IsCanonical) return;
        if (Creature is not { IsAlive: true }) return;

        foreach (var dynamicVar in DynamicVars.Values)
            switch (dynamicVar)
            {
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
                    snd.PreviewValue =
                        SlimeBossHook.ModifySecondarySlimeEffects(CombatState, snd.IntValue, out _, this);
                    break;
            }
    }
}

public enum SlimeType
{
    Single,
    Counter
}