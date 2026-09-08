using Champ.ChampCode.Cards.Basic;
using Champ.ChampCode.Relics;
using Champ.ChampCode.Stance;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Config;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Core;

#pragma warning disable STS001
public class Champ : DownfallCharacterModel
#pragma warning restore STS001
{
    private static readonly Color Color = new(0x5E594FFF);
    public override Color EnergyLabelOutlineColor => new(0x464203FF);
    public override string CharId => "Champ";
    public override string ModId => ChampMainFile.ModId;
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override float CardColorH => 0.6f;
    public override float CardColorS => 0.5f;
    public override float CardColorV => 1.2f;
    public override Color MapDrawingColor => Color;

    public override bool HideFromVanillaCharacterSelect => DownfallConfig.HideChamp;
    public override bool HideInCompendium => DownfallConfig.HideChamp;

    public override CharacterGender Gender => CharacterGender.Masculine;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 80;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeChamp>(),
        ModelDb.Card<StrikeChamp>(),
        ModelDb.Card<StrikeChamp>(),
        ModelDb.Card<StrikeChamp>(),
        ModelDb.Card<DefendChamp>(),
        ModelDb.Card<DefendChamp>(),
        ModelDb.Card<DefendChamp>(),
        ModelDb.Card<DefendChamp>(),
        ModelDb.Card<BerserkersShout>(),
        ModelDb.Card<DefensiveShout>(),
        ModelDb.Card<Execute>()
    ];


    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<ChampionsCrown>()
    ];

    public override float AttackAnimDelay => 0.2f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<ChampCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<ChampPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<ChampRelicPool>();

    public static string GetJumpAnimIfApplicable(CharacterModel character)
    {
        return character is not Champ ? "Attack" : "jumpAttack";
    }

    public static float GetJumpAttackDelayIfApplicable(CharacterModel character)
    {
        return character is not Champ ? character.AttackAnimDelay : 0.5f;
    }

    
    protected override List<(AnimState, string)> AnimationStates =>
        base.AnimationStates.Concat([
            (new AnimState("attack_jump"),  "jumpAttack")
        ]).ToList();
    
    private Func<Creature, ChampStanceModel?> Stance => creature => creature.Player == null ? null : ChampModel.GetStanceModel(creature.Player);


    public override CreatureAnimator GenerateAnimator(MegaSprite controller, Creature creature)
    {
        var idle = new AnimState("idle_loop", true);
        var idleBerserker = new AnimState("idle_loop_berserker", true);
        var idleDefensive = new AnimState("idle_loop_defensive", true);
        var idleUltimate = new AnimState("idle_loop_ultimate", true);

        var idles = new (string name, AnimState state, Func<bool> when)[]
        {
            ("IdleUltimate", idleUltimate, () => Stance(creature) is ChampUltimateStance),
            ("IdleDefensive", idleDefensive, () => Stance(creature) is ChampDefensiveStance),
            ("IdleBerserker", idleBerserker, () => Stance(creature) is ChampBerserkerStance),
            ("Idle", idle, () => Stance(creature) is ChampNoStance or null),
        };

        var animator = new CreatureAnimator(PickIdle(), controller);

        foreach (var (name, state, when) in idles)
            animator.AddAnyState(name, state, when);

        foreach (var (animState, trigger) in AnimationStates)
        {
            if (trigger == CreatureAnimator.hitTrigger)
                continue;
            foreach (var (_, state, when) in idles)
                animState.AddNextState(state, when);
            animator.AddAnyState(trigger, animState);
        }
        
        var hurts = new (AnimState state, Func<bool> when)[]
        {
            (new AnimState("hurt_berserker"), () => Stance(creature) is ChampUltimateStance),
            (new AnimState("hurt_defensive"), () => Stance(creature) is ChampDefensiveStance),
            (new AnimState("hurt_berserker"), () => Stance(creature) is ChampBerserkerStance),
            (new AnimState("hurt"), () => Stance(creature) is ChampNoStance),
        };

        foreach (var (hurtState, hurtWhen) in hurts)
        {
            foreach (var (_, idleState, idleWhen) in idles)
                hurtState.AddNextState(idleState, idleWhen);

            animator.AddAnyState(CreatureAnimator.hitTrigger, hurtState, hurtWhen);
        }

        animator.AddAnyState("Dead", new AnimState("die"));
        animator.AddAnyState("Relaxed", new AnimState("relaxed_loop", true));
        return animator;

        AnimState PickIdle() => idles.First(i => i.when()).state;
    }
}

public class ChampRelicPool : DownfallRelicPool<Champ>;

public abstract class ChampRelicModel(RelicRarity rarity, bool autoAdd = true)
    : DownfallRelicModel<Champ>(rarity, autoAdd);

public abstract class ChampPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Champ>(powerType, powerStackType);

public class ChampPotionPool : DownfallPotionPool<Champ>;

public class ChampCardPool : DownfallCardPool<Champ>;

public abstract class ChampPotionModel(PotionRarity potionRarity, PotionUsage potionUsage, TargetType targetType) :
    DownfallPotionModel<Champ>(potionRarity, potionUsage, targetType);