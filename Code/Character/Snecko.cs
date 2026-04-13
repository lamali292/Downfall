using Downfall.Code.Abstract;
using Downfall.Code.Cards.Snecko.Basic;
using Downfall.Code.Relics.Snecko;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Downfall.Code.Character;

public class Snecko : DownfallCharacterModel
{
    private static readonly Color Color = new(0x467A94FF);
    public override string CharId => "Snecko";
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

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        GD.Print("[Downfall] GenerateAnimator called");

        var animState = new AnimState("Idle", true);
        var state1 = new AnimState("Idle");
        var state2 = new AnimState("Attack");
        var state3 = new AnimState("Hit");
        var state4 = new AnimState("Idle");
        var state5 = new AnimState("Idle");
        state1.NextState = animState;
        state2.NextState = animState;
        state3.NextState = animState;
        state5.NextState = animState;
        state5.AddBranch("Idle", animState);
        var animator = new CreatureAnimator(animState, controller);
        animator.AddAnyState("Idle", animState);
        animator.AddAnyState("Dead", state4);
        animator.AddAnyState("Hit", state3);
        animator.AddAnyState("Attack", state2);
        animator.AddAnyState("Cast", state1);
        animator.AddAnyState("Relaxed", state5);

        return animator;
    }
}