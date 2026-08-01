using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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

public abstract class SlimeModel : CustomMonsterModel
{
    private DynamicVarSet? _dynamicVars;
    public override int MinInitialHp => Really.bigNumber;
    public override int MaxInitialHp => Really.bigNumber;
    public abstract SlimeType SlimeType { get; }

    public override string CustomVisualPath =>
        $"combat/{Id.Entry.RemovePrefix().ToLowerInvariant()}.tscn".SlimeScenePath();

    public override bool HasDeathSfx => false;
    public Creature PetOwner => Creature.PetOwner?.Creature ?? throw new ArgumentNullException(nameof(PetOwner));
    protected virtual LocString Description => L10NMonsterLookup(Id.Entry + ".description");

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
    
    protected override void DeepCloneFields()
    {
        _dynamicVars = DynamicVars.Clone(this);
    }


    protected virtual IEnumerable<DynamicVar> CanonicalVars => [];

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var initialState = new MoveState("NOTHING_MOVE", _ => Task.CompletedTask);
        initialState.FollowUpState = initialState;
        return new MonsterMoveStateMachine([initialState], initialState);
    }

    public abstract Task Command(PlayerChoiceContext ctx);


    protected virtual void UpdatePreviewValues()
    {
        if (IsCanonical || _creature == null) return;
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

[Flags]
public enum SlimeType
{
    None = 0,
    Normal = 1,
    Specialist = 2,
    Any = Normal | Specialist
}