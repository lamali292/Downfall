using Downfall.DownfallCode.Abstract;
using Godot;
using Hexaghost.HexaghostCode.Cards.Basic;
using Hexaghost.HexaghostCode.Relics;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Core;

public class Hexaghost : DownfallCharacterModel
{
    private static readonly Color Color = new(0x723E6DFF);
    public override string ModId => HexaghostMainFile.ModId;
    public override string CharId => "Hexaghost";
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override Color CardColor => Color;
    public override Color MapDrawingColor => Color;

    public override CharacterGender Gender => CharacterGender.Neutral;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 66;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeHexaghost>(),
        ModelDb.Card<StrikeHexaghost>(),
        ModelDb.Card<StrikeHexaghost>(),
        ModelDb.Card<StrikeHexaghost>(),
        ModelDb.Card<DefendHexaghost>(),
        ModelDb.Card<DefendHexaghost>(),
        ModelDb.Card<DefendHexaghost>(),
        ModelDb.Card<DefendHexaghost>(),
        ModelDb.Card<Sear>(),
        ModelDb.Card<Kindle>()
    ];


    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<SpiritBrand>()
    ];


    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<HexaghostCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<HexaghostPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<HexaghostRelicPool>();
}

public class HexaghostRelicPool : DownfallRelicPool<Hexaghost>;

public abstract class HexaghostRelicModel : DownfallRelicModel<Hexaghost>;

public abstract class HexaghostPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Hexaghost>(powerType, powerStackType);

public class HexaghostPotionPool : DownfallPotionPool<Hexaghost>;

public class HexaghostCardPool : DownfallCardPool<Hexaghost>;

public class HexaghostChoiceCardPool : DownfallCardPool<Hexaghost>
{
    public override bool IsShared => true;
}