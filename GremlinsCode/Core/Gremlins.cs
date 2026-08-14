using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Config;

using Godot;
using Gremlins.GremlinsCode.Cards.Basic;
using Gremlins.GremlinsCode.Relics;
using Gremlins.GremlinsCode.Vfx;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Gremlins.GremlinsCode.Core;

public class Gremlins : DownfallCharacterModel
{
    private static readonly Color Color = new(0xCA5B5BFF);
    public override Color EnergyLabelOutlineColor  => new(0x6f0060FF);
    public override string CharId => "Gremlins";
    public override string ModId => GremlinsMainFile.ModId;
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override float CardColorH => 0.8f;
    public override float CardColorS => 0.4f;
    public override float CardColorV => 1.2f;
    public override Color MapDrawingColor => Color;

    public override CharacterGender Gender => CharacterGender.Neutral;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 16;
    public override int StartingGold => 99;


    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeGremlins>(),
        ModelDb.Card<StrikeGremlins>(),
        ModelDb.Card<StrikeGremlins>(),
        ModelDb.Card<StrikeGremlins>(),
        ModelDb.Card<DefendGremlins>(),
        ModelDb.Card<DefendGremlins>(),
        ModelDb.Card<DefendGremlins>(),
        ModelDb.Card<DefendGremlins>(),
        ModelDb.Card<GremlinDance>(),
        ModelDb.Card<TagTeam>()
    ];


    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<MobLeadersStaff>()
    ];

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<GremlinsCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<GremlinsPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<GremlinsRelicPool>();
    
    public override NCreatureVisuals? CreateCustomVisuals()
    {
        return GD.Load<PackedScene>("res://Gremlins/scenes/character/combat.tscn")
            .Instantiate<NGremlinsCreatureVisuals>();
    }
}

public class GremlinsRelicPool : DownfallRelicPool<Gremlins>;

public abstract class GremlinsRelicModel(RelicRarity rarity, bool autoAdd = true) : DownfallRelicModel<Gremlins>(rarity, autoAdd);

public abstract class GremlinsPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Gremlins>(powerType, powerStackType);

public abstract class GremlinsCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool showInCardLibrary = true,
    bool autoAdd = true)
    : DownfallCardModel<Gremlins>(cost, type, rarity, targetType, showInCardLibrary, autoAdd);

public class GremlinsPotionPool : DownfallPotionPool<Gremlins>;

public class GremlinsCardPool : DownfallCardPool<Gremlins>;