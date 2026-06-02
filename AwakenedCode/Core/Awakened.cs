using Awakened.AwakenedCode.Cards.Basic;
using Awakened.AwakenedCode.Relics;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Utils.Sound;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Core;

public class Awakened : DownfallCharacterModel
{
    private static readonly Color Color = new(0x12FAF0FF);
    public override Color EnergyLabelOutlineColor  => new(0x004956FF);
    public override string CharId => "Awakened";
    public override string ModId => AwakenedMainFile.ModId;
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override float CardColorH => 0.6f;
    public override float CardColorS => 0.5f;
    public override float CardColorV => 1f;
    public override Color MapDrawingColor => Color;

    public override CharacterGender Gender => CharacterGender.Neutral;
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


    public override ModSoundEffect CharacterSelectSfxEntry => new(
        new ModSoundEntry("res://Awakened/audio/chant_activatev2.ogg", 1, 0.1f, 1, 10)
    );

    /*
    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        var idleState = new AnimState("Idle_1", true);
        var hitState = new AnimState("Hit");
        var attackState = new AnimState("Attack_1");
        var awakenedIdle = new AnimState("Idle_2", true);
        var awakenedAttack = new AnimState("Attack_2");
        var awakenedHit = new AnimState("Hit");

        var animator = new CreatureAnimator(idleState, controller);
        animator.AddAnyState("Idle", idleState, () => !IsAwakened());
        animator.AddAnyState("Idle", awakenedIdle, IsAwakened);
        animator.AddAnyState("Attack", attackState, () => !IsAwakened());
        animator.AddAnyState("Attack", awakenedAttack, IsAwakened);
        animator.AddAnyState("Hit", hitState, () => !IsAwakened());
        animator.AddAnyState("Hit", awakenedHit, IsAwakened);

        return animator;

        bool IsAwakened()
        {
            return AwakenedModel
                .IsAwakened(CombatManager.Instance.DebugOnlyGetState()?.Players
                    .FirstOrDefault(p => p.Character == this));
        }
    }
    */
}

public class AwakenedRelicPool : DownfallRelicPool<Awakened>;

public abstract class AwakenedRelicModel(RelicRarity rarity, bool autoAdd = true) : DownfallRelicModel<Awakened>(rarity, autoAdd);

public abstract class AwakenedPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Awakened>(powerType, powerStackType);

public class AwakenedPotionPool : DownfallPotionPool<Awakened>;

public class AwakenedCardPool : DownfallCardPool<Awakened>;