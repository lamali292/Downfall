using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Utils.Sound;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
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
    public override Color EnergyLabelOutlineColor => new(0x005704FF);
    public override string CharId => "SlimeBoss";
    public override string ModId => SlimeBossMainFile.ModId;
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override float CardColorH => 0.45f;
    public override float CardColorS => 0.5f;
    public override float CardColorV => 1.2f;
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
    
    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<HeartOfGoo>()
    ];

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<SlimeBossCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<SlimeBossPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<SlimeBossRelicPool>();
}

public class SlimeBossRelicPool : DownfallRelicPool<SlimeBoss>;

public abstract class SlimeBossRelicModel(RelicRarity rarity, bool autoAdd = true)
    : DownfallRelicModel<SlimeBoss>(rarity, autoAdd);

public abstract class SlimeBossPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<SlimeBoss>(powerType, powerStackType);

public abstract class SlimeBossCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool showInCardLibrary = true,
    bool autoAdd = true)
    : DownfallCardModel<SlimeBoss>(cost, type, rarity, targetType, showInCardLibrary, autoAdd);

public class SlimeBossPotionPool : DownfallPotionPool<SlimeBoss>;

public class SlimeBossCardPool : DownfallCardPool<SlimeBoss>;