using Downfall.Code.Abstract;
using Downfall.Code.Cards.Collector.Basic;
using Downfall.Code.Relics.Collector;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Character;

public class Collector : DownfallCharacterModel
{
    private static readonly Color Color = new(0x0D9D82FF);
    public override string CharId => "Collector";
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override Color CardColor => Color;
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