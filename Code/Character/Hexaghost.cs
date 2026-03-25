using Downfall.Code.Abstract;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Downfall.Code.Character;

public class Hexaghost : DownfallCharacterModel
{
    private static readonly Color Color = StsColors.purple;
    public override string CharId => "Hexaghost";
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

    public override CardPoolModel CardPool => ModelDb.CardPool<HexaghostCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<HexaghostPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<HexaghostRelicPool>();
}