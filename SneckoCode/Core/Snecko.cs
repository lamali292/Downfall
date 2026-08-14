using Downfall.DownfallCode.Abstract;

using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Cards.Basic;
using Snecko.SneckoCode.Relics;

namespace Snecko.SneckoCode.Core;

public class Snecko : DownfallCharacterModel
{
    private static readonly Color Color = new(0x467A94FF);
    public override Color EnergyLabelOutlineColor => new(0x317394FF);
    public override string CharId => "Snecko";
    public override string ModId => SneckoMainFile.ModId;
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override float CardColorH => 0.55f;
    public override float CardColorS => 0.5f;
    public override float CardColorV => 1.3f;
    public override Color MapDrawingColor => Color;

    public override CharacterGender Gender => CharacterGender.Neutral;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 85;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeSnecko>(),
        ModelDb.Card<StrikeSnecko>(),
        ModelDb.Card<StrikeSnecko>(),
        ModelDb.Card<StrikeSnecko>(),
        ModelDb.Card<DefendSnecko>(),
        ModelDb.Card<DefendSnecko>(),
        ModelDb.Card<DefendSnecko>(),
        ModelDb.Card<DefendSnecko>(),
        ModelDb.Card<SnekBite>(),
        ModelDb.Card<TailWhip>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<SneckoSoul>()
    ];

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<SneckoCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<SneckoPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<SneckoRelicPool>();
}

public class SneckoRelicPool : DownfallRelicPool<Snecko>;

public abstract class SneckoRelicModel(RelicRarity rarity, bool autoAdd = true)
    : DownfallRelicModel<Snecko>(rarity, autoAdd);

public abstract class SneckoPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Snecko>(powerType, powerStackType);

public class SneckoPotionPool : DownfallPotionPool<Snecko>;

public class SneckoCardPool : DownfallCardPool<Snecko>;