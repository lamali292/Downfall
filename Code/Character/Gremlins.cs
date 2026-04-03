using Downfall.Code.Abstract;
using Downfall.Code.Cards.Gremlins.Basic;
using Downfall.Code.Nodes;
using Downfall.Code.Relics.Gremlins;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.Code.Character;

public class Gremlins : DownfallCharacterModel
{
    private static readonly Color Color = new(0xCA5B5BFF);
    public override string CharId => "Gremlins";
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override Color CardColor => Color;
    public override Color MapDrawingColor => Color;

    public override CharacterGender Gender => CharacterGender.Neutral;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 72;
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
        return GD.Load<PackedScene>("res://Downfall/character/scenes/combat_scene/gremlins_combat.tscn")
            .Instantiate<NGremlinsCreatureVisuals>();
    }
}