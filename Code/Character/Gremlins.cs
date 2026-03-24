using Downfall.Code.Abstract;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Downfall.Code.Character;

public class Gremlins : DownfallCharacterModel<Gremlins>
{
    private static readonly Color Color = StsColors.purple;
    public override string CharId => "Gremlins";
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override Color CardHsv => new(0.75f, 1f, 1f);

    public override CharacterGender Gender => CharacterGender.Neutral;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 72;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<Void>(),
        ModelDb.Card<Void>(),
        ModelDb.Card<Void>(),
        ModelDb.Card<Void>(),
        ModelDb.Card<Void>(),
        ModelDb.Card<Void>(),
        ModelDb.Card<Void>(),
        ModelDb.Card<Void>(),
        ModelDb.Card<Void>(),
        ModelDb.Card<Void>()
    ];


    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<TungstenRod>()
    ];

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<GremlinsCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<GremlinsPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<GremlinsRelicPool>();
}