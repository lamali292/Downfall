using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Utils.Sound;
using Godot;
using Hermit.HermitCode.Cards.Basic;
using Hermit.HermitCode.Relics;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Core;

public class Hermit : DownfallCharacterModel
{
    private static readonly Color Color = new(0xCEA477FF);
    public override Color EnergyLabelOutlineColor  => new(0xBA8900FF);
    public override string CharId => "Hermit";
    public override string ModId => HermitMainFile.ModId;
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override float CardColorH => 0.1f;
    public override float CardColorS => 0.4f;
    public override float CardColorV => 1.2f;
    public override Color MapDrawingColor => new(0x7A4900FF);

    public override CharacterGender Gender => CharacterGender.Neutral;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 70;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeHermit>(),
        ModelDb.Card<StrikeHermit>(),
        ModelDb.Card<StrikeHermit>(),
        ModelDb.Card<StrikeHermit>(),
        ModelDb.Card<DefendHermit>(),
        ModelDb.Card<DefendHermit>(),
        ModelDb.Card<DefendHermit>(),
        ModelDb.Card<DefendHermit>(),
        ModelDb.Card<Covet>(),
        ModelDb.Card<Snapshot>()
    ];

    public override ModSoundEffect CharacterSelectSfxEntry => new(
        new ModSoundEntry("res://Hermit/audio/hermit_gun.ogg", 10, 0.3f, 1, 8),
        new ModSoundEntry("res://Hermit/audio/hermit_gun2.ogg", 3, 0.3f, 1, 8),
        new ModSoundEntry("res://Hermit/audio/hermit_gun3.ogg", 1, 0.3f, 1, 8),
        new ModSoundEntry("res://Hermit/audio/hermit_reload.ogg", 6, 0.3f, 1, 8),
        new ModSoundEntry("res://Hermit/audio/hermit_spin.ogg", 4, 0.3f, 1, 8)
    );


    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<OldLocket>()
    ];

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<HermitCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<HermitPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<HermitRelicPool>();

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        GD.Print("[Downfall] GenerateAnimator called");

        var animState = new AnimState("Idle", true);
        var state1 = new AnimState("Idle");
        var state2 = new AnimState("Attack");
        var state3 = new AnimState("Hit");
        var state4 = new AnimState("Idle");
        var state5 = new AnimState("Idle");
        state1.NextState = animState;
        state2.NextState = animState;
        state3.NextState = animState;
        state5.NextState = animState;
        state5.AddBranch("Idle", animState);
        var animator = new CreatureAnimator(animState, controller);
        animator.AddAnyState("Idle", animState);
        animator.AddAnyState("Dead", state4);
        animator.AddAnyState("Hit", state3);
        animator.AddAnyState("Attack", state2);
        animator.AddAnyState("Cast", state1);
        animator.AddAnyState("Relaxed", state5);

        return animator;
    }
}

public class HermitRelicPool : DownfallRelicPool<Hermit>;

[Pool(typeof(HermitRelicPool))]
public abstract class HermitRelicModel(RelicRarity rarity, bool autoAdd = true) : DownfallRelicModel<Hermit>(rarity, autoAdd);

public abstract class HermitPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Hermit>(powerType, powerStackType);

public class HermitPotionPool : DownfallPotionPool<Hermit>;

public class HermitCardPool : DownfallCardPool<Hermit>;

[Pool(typeof(HermitPotionPool))]
public abstract class HermitPotionModel(PotionRarity potionRarity, PotionUsage potionUsage, TargetType targetType) :
    DownfallPotionModel<Hermit>(potionRarity, potionUsage, targetType);