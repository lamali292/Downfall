using Downfall.Code.Abstract;
using Downfall.Code.Cards.Awakened.Basic;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Downfall.Code.Character;

public class Awakened : DownfallCharacterModel<Awakened>
{
    private static readonly Color Color = StsColors.purple;
    public override string CharId => "Awakened";
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override Color CardHsv => new(0.64f, 0.5f, 1f);

    public override CharacterGender Gender => CharacterGender.Neutral;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 72;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeAwakened>(),
        ModelDb.Card<StrikeAwakened>(),
        ModelDb.Card<StrikeAwakened>(),
        ModelDb.Card<StrikeAwakened>(),
        ModelDb.Card<DefendAwakened>(),
        ModelDb.Card<DefendAwakened>(),
        ModelDb.Card<DefendAwakened>(),
        ModelDb.Card<DefendAwakened>(),
        ModelDb.Card<Void>(),
        ModelDb.Card<Void>()
    ];


    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<TungstenRod>()
    ];

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<AwakenedCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<AwakenedPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<AwakenedRelicPool>();

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        GD.Print("[Downfall] GenerateAnimator called");

        var animState = new AnimState("Idle_1", true);
        var state1 = new AnimState("Idle_1");
        var state2 = new AnimState("Attack_1");
        var state3 = new AnimState("Hit");
        var state4 = new AnimState("Idle_1");
        var state5 = new AnimState("Idle_1");
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