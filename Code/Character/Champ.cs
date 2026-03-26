using Downfall.Code.Abstract;
using Downfall.Code.Cards.Champ.Basic;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Downfall.Code.Character;

public class Champ : DownfallCharacterModel
{
    private static readonly Color Color = StsColors.purple;
    public override string CharId => "Champ";
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override Color CardColor => Color;
    public override Color MapDrawingColor => Color;

    public override CharacterGender Gender => CharacterGender.Masculine;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 72;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeChamp>(),
        ModelDb.Card<StrikeChamp>(),
        ModelDb.Card<StrikeChamp>(),
        ModelDb.Card<StrikeChamp>(),
        ModelDb.Card<DefendChamp>(),
        ModelDb.Card<DefendChamp>(),
        ModelDb.Card<DefendChamp>(),
        ModelDb.Card<BerserkersShout>(),
        ModelDb.Card<DefensiveShout>(),
        ModelDb.Card<Execute>()
    ];


    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<TungstenRod>()
    ];

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<ChampCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<ChampPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<ChampRelicPool>();

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