using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Config;
using Godot;
using Guardian.GuardianCode.Cards.Basic;
using Guardian.GuardianCode.Relics;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Core;

public class Guardian : DownfallCharacterModel
{
    private static readonly Color Color = new(0xCA5B5BFF);
    public override Color EnergyLabelOutlineColor => new(0x575044FF);
    public override string CharId => "Guardian";
    public override string ModId => GuardianMainFile.ModId;
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override float CardColorH => 0.17f;
    public override float CardColorS => 1.5f;
    public override float CardColorV => 1.2f;
    public override Color MapDrawingColor => Color;

    public override bool HideFromVanillaCharacterSelect => DownfallConfig.HideGuardian;
    public override bool HideInCompendium => DownfallConfig.HideGuardian;
    
    public override CharacterGender Gender => CharacterGender.Neutral;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 80;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeGuardian>(),
        ModelDb.Card<StrikeGuardian>(),
        ModelDb.Card<StrikeGuardian>(),
        ModelDb.Card<StrikeGuardian>(),
        ModelDb.Card<DefendGuardian>(),
        ModelDb.Card<DefendGuardian>(),
        ModelDb.Card<DefendGuardian>(),
        ModelDb.Card<DefendGuardian>(),
        ModelDb.Card<CurlUp>(),
        ModelDb.Card<TwinSlam>()
    ];

    protected override IEnumerable<string> ExtraAssetPaths =>
        GuardianModelDb.AllGems.Select(g => g.IconPath);

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<BronzeGear>()
    ];

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<GuardianCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<GuardianPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<GuardianRelicPool>();


    private Func<Creature, bool> IsDefensive => creature => creature.Player != null && GuardianCmd.IsInMode<GuardianDefensiveMode>(creature.Player);

    public override CreatureAnimator GenerateAnimator(MegaSprite controller, Creature creature)
    {
        var idle          = new AnimState("idle_loop", true);
        var idleDefensive = new AnimState("idle_loop_defensive", true);

        var idles = new (string name, AnimState state, Func<bool> when)[]
        {
            ("IdleDefensive", idleDefensive, () => IsDefensive(creature)),
            ("Idle",          idle,          () => !IsDefensive(creature)),
        };

        var animator = new CreatureAnimator(PickIdle(), controller);

        foreach (var (name, state, when) in idles)
            animator.AddAnyState(name, state, when);

        var attack          = new AnimState("attack");
        var attackDefensive = new AnimState("attack_defensive");
        var hurt            = new AnimState("hurt");
        var hurtDefensive   = new AnimState("hurt_defensive");

       
        var attacks = new (AnimState state, Func<bool> when)[]
        {
            (attackDefensive, () => IsDefensive(creature)),
            (attack,          () => !IsDefensive(creature)),
        };

        
        var transitionIn   = new AnimState("transition_in");
        foreach (var (_, idleState, idleWhen) in idles)
            transitionIn.AddNextState(idleState, idleWhen);
        animator.AddAnyState("TransitionIn", transitionIn);
        
        var transitionOut   = new AnimState("transition_out");
        foreach (var (_, idleState, idleWhen) in idles)
            transitionOut.AddNextState(idleState, idleWhen);
        animator.AddAnyState("TransitionOut", transitionOut);
        
        foreach (var (state, when) in attacks)
        {
            foreach (var (_, idleState, idleWhen) in idles)
                state.AddNextState(idleState, idleWhen);
            animator.AddAnyState(CreatureAnimator.attackTrigger, state, when);
        }

        var hurts = new (AnimState state, Func<bool> when)[]
        {
            (hurtDefensive, () => IsDefensive(creature)),
            (hurt,          () => !IsDefensive(creature)),
        };

        foreach (var (state, when) in hurts)
        {
            foreach (var (_, idleState, idleWhen) in idles)
                state.AddNextState(idleState, idleWhen);
            animator.AddAnyState(CreatureAnimator.hitTrigger, state, when);
        }

        animator.AddAnyState(CreatureAnimator.deathTrigger, new AnimState("die"));
        return animator;

        AnimState PickIdle() => idles.First(i => i.when()).state;
    }

}

public class GuardianRelicPool : DownfallRelicPool<Guardian>;

public abstract class GuardianRelicModel(RelicRarity rarity, bool autoAdd = true)
    : DownfallRelicModel<Guardian>(rarity, autoAdd);

public abstract class GuardianPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Guardian>(powerType, powerStackType);

public class GuardianPotionPool : DownfallPotionPool<Guardian>;

public class GuardianCardPool : DownfallCardPool<Guardian>;

public abstract class GuardianPotionModel(PotionRarity potionRarity, PotionUsage potionUsage, TargetType targetType) :
    DownfallPotionModel<Guardian>(potionRarity, potionUsage, targetType);