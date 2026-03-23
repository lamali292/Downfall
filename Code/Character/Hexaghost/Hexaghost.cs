using Downfall.Code.Character.Abstract;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Downfall.Code.Character.Hexaghost;

public class Hexaghost : DownfallCharacterModel
{
    public const string CharacterId = "Hexaghost";
    public static readonly Color Color = StsColors.purple;
    protected override string CharId => CharacterId;
    public override Color NameColor => Color;

    public override CharacterGender Gender => CharacterGender.Feminine;
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
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<HexaghostRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<HexaghostPotionPool>();
}