using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Utils.Sound;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Cards.Basic;
using SlimeBoss.SlimeBossCode.Relics;

namespace SlimeBoss.SlimeBossCode.Core;

public class SlimeBoss : DownfallCharacterModel
{
    private static readonly Color Color = new(0x195E19FF);
    public override string CharId => "SlimeBoss";
    public override string ModId => SlimeBossMainFile.ModId;
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override Color CardColor => Color;
    public override Color MapDrawingColor => Color;

    public override CharacterGender Gender => CharacterGender.Neutral;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 65;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeSlimeBoss>(),
        ModelDb.Card<StrikeSlimeBoss>(),
        ModelDb.Card<StrikeSlimeBoss>(),
        ModelDb.Card<DefendSlimeBoss>(),
        ModelDb.Card<DefendSlimeBoss>(),
        ModelDb.Card<DefendSlimeBoss>(),
        ModelDb.Card<DefendSlimeBoss>(),
        ModelDb.Card<CorrosiveSpit>(),
        ModelDb.Card<Split>(),
        ModelDb.Card<Tackle>()
    ];


    public override ModSoundEffect CharacterSelectSfxEntry => new(
        new ModSoundEntry("res://SlimeBoss/audio/character_select/SOTE_SFX_SlimeSplit_v1.ogg", 1, 0.3f, 1, 10)
    );

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<HeartOfGoo>()
    ];

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<SlimeBossCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<SlimeBossPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<SlimeBossRelicPool>();

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        GD.Print("[Downfall] GenerateAnimator called");

        var animState = new AnimState("idle", true);
        var state1 = new AnimState("idle");
        var state2 = new AnimState("idle");
        var state3 = new AnimState("idle");
        var state4 = new AnimState("idle");
        var state5 = new AnimState("idle");
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

public class SlimeBossRelicPool : DownfallRelicPool<SlimeBoss>;

public abstract class SlimeBossRelicModel(RelicRarity rarity) : DownfallRelicModel<SlimeBoss>(rarity);

public abstract class SlimeBossPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<SlimeBoss>(powerType, powerStackType);

public class SlimeBossPotionPool : DownfallPotionPool<SlimeBoss>;

public class SlimeBossCardPool : DownfallCardPool<SlimeBoss>;