using BaseLib.Abstracts;
using Collector.CollectorCode.Cards.Basic;
using Collector.CollectorCode.Relics;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Utils.Sound;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Core;

public class Collector : DownfallCharacterModel
{
    private static readonly Color Color = new(0x0D9D82FF);
    public override Color EnergyLabelOutlineColor  => new(0x004f04FF);
    public override string CharId => "Collector";
    public override string ModId => CollectorMainFile.ModId;
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override float CardColorH => 0.25f;
    public override float CardColorS => 0.6f;
    public override float CardColorV => 1f;
    public override Color MapDrawingColor => Color;

    public override CharacterGender Gender => CharacterGender.Feminine;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 65;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeCollector>(),
        ModelDb.Card<StrikeCollector>(),
        ModelDb.Card<StrikeCollector>(),
        ModelDb.Card<StrikeCollector>(),
        ModelDb.Card<DefendCollector>(),
        ModelDb.Card<DefendCollector>(),
        ModelDb.Card<DefendCollector>(),
        ModelDb.Card<DefendCollector>(),
        ModelDb.Card<FuelTheFire>(),
        ModelDb.Card<YouAreMine>()
    ];


    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<EmeraldTorch>()
    ];

    public override ModSoundEffect CharacterSelectSfxEntry => new(
        new ModSoundEntry("res://Collector/audio/character_select/STS_SFX_CollectorSummon_v2.ogg", 1, 0.1f, 1, 7)
    );

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<CollectorCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<CollectorPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<CollectorRelicPool>();


    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        return SetupAnimationState(
            controller,
            "idle",
            hitName: "Hit"
        );
    }
}

public class CollectorRelicPool : DownfallRelicPool<Collector>;

public abstract class CollectorRelicModel(RelicRarity rarity, bool autoAdd = true) : DownfallRelicModel<Collector>(rarity, autoAdd);

public abstract class CollectorPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Collector>(powerType, powerStackType);

public class CollectorPotionPool : DownfallPotionPool<Collector>;

public class CollectorCardPool : DownfallCardPool<Collector>;

public class CollectibleCardPool : CustomCardPoolModel
{
    private static readonly Color Color = new("C6C1FF");
    public override string Title => "Collectible";

    public override string BigEnergyIconPath => ModelDb.CardPool<CollectorCardPool>().BigEnergyIconPath;
    public override string TextEnergyIconPath => ModelDb.CardPool<CollectorCardPool>().TextEnergyIconPath;

    public override float H => Color.H;
    public override float S => Color.S;
    public override float V => Color.V;


    public override Color DeckEntryCardColor => Color;
    public override bool IsColorless => false;
    public override bool IsShared => true;
}