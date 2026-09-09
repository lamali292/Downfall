using Awakened.AwakenedCode.Cards.Basic;
using Awakened.AwakenedCode.Relics;
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

namespace Awakened.AwakenedCode.Core;

public class Awakened : DownfallCharacterModel
{
    private static readonly Color Color = new(0x12FAF0FF);
    public override Color EnergyLabelOutlineColor => new(0x004956FF);
    public override string CharId => "Awakened";
    public override string ModId => AwakenedMainFile.ModId;
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override float CardColorH => 0.6f;
    public override float CardColorS => 0.5f;
    public override float CardColorV => 1f;
    public override Color MapDrawingColor => Color;

    public override bool HideFromVanillaCharacterSelect => DownfallConfig.HideAwakened;
    public override bool HideInCompendium => DownfallConfig.HideAwakened;
    
    public override CharacterGender Gender => CharacterGender.Masculine;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 70;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeAwakened>(),
        ModelDb.Card<StrikeAwakened>(),
        ModelDb.Card<StrikeAwakened>(),
        ModelDb.Card<StrikeAwakened>(),
        ModelDb.Card<DefendAwakened>(),
        ModelDb.Card<DefendAwakened>(),
        ModelDb.Card<DefendAwakened>(),
        ModelDb.Card<DefendAwakened>(),
        ModelDb.Card<Hymn>(),
        ModelDb.Card<TalonRake>()
    ];


    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<RippedDoll>()
    ];

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<AwakenedCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<AwakenedPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<AwakenedRelicPool>();


    private Func<Creature, bool> IsAwakened => creature => AwakenedModel.IsAwakened(creature.Player);

    public override CreatureAnimator GenerateAnimator(MegaSprite controller, Creature creature)
    {
        var idle            = new AnimState("idle_loop", true);
        var idleLow         = new AnimState("low_health_loop", true);
        var idleAwakened    = new AnimState("idle_loop_awakened", true);
        var idleAwakenedLow = new AnimState("low_health_loop_awakened", true);
        
        var idles = new (string name, AnimState state, Func<bool> when)[]
        {
            ("IdleAwakenedLow", idleAwakenedLow, () =>  IsAwakened(creature) &&  IsLowHealth(creature)),
            ("IdleAwakened",    idleAwakened,    () =>  IsAwakened(creature) && !IsLowHealth(creature)),
            ("IdleLow",         idleLow,         () => !IsAwakened(creature) &&  IsLowHealth(creature)),
            ("Idle",            idle,            () =>  !IsAwakened(creature) &&  !IsLowHealth(creature))
        };
        
        var animator = new CreatureAnimator(PickIdle(), controller);

        foreach (var (name, state, when) in idles)
            animator.AddAnyState(name, state, when);

        foreach (var (animState, trigger) in AnimationStates)
        {
            foreach (var (_, state, when) in idles)
                animState.AddNextState(state, when);
            animator.AddAnyState(trigger, animState);
        }

        animator.AddAnyState("Dead", new AnimState("die"));
        animator.AddAnyState("Relaxed", new AnimState("relaxed_loop", true));
        return animator;

        AnimState PickIdle() => idles.First(i => i.when()).state;
    }
  
}

public class AwakenedRelicPool : DownfallRelicPool<Awakened>;

public abstract class AwakenedRelicModel(RelicRarity rarity, bool autoAdd = true)
    : DownfallRelicModel<Awakened>(rarity, autoAdd);

public abstract class AwakenedPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Awakened>(powerType, powerStackType);

public class AwakenedPotionPool : DownfallPotionPool<Awakened>;

public class AwakenedCardPool : DownfallCardPool<Awakened>;

public abstract class AwakenedPotionModel(PotionRarity potionRarity, PotionUsage potionUsage, TargetType targetType) :
    DownfallPotionModel<Awakened>(potionRarity, potionUsage, targetType);