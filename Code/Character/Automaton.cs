using Downfall.Code.Abstract;
using Downfall.Code.Cards.Automaton.Basic;
using Downfall.Code.Relics.Automaton;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Downfall.Code.Character;

public class Automaton : DownfallCharacterModel
{
    private static readonly Color Color = new(0xD4C99DFF);
    public override string CharId => "Automaton";
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override Color CardColor => Color;
    public override Color MapDrawingColor => Color;

    public override CharacterGender Gender => CharacterGender.Feminine;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 72;
    public override int StartingGold => 99;

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<BronzeCore>()
    ];

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<AutomatonCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<AutomatonPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<AutomatonRelicPool>();

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeAutomaton>(),
        ModelDb.Card<StrikeAutomaton>(),
        ModelDb.Card<StrikeAutomaton>(),
        ModelDb.Card<StrikeAutomaton>(),
        ModelDb.Card<DefendAutomaton>(),
        ModelDb.Card<DefendAutomaton>(),
        ModelDb.Card<DefendAutomaton>(),
        ModelDb.Card<DefendAutomaton>(),
        ModelDb.Card<Goto>(),
        ModelDb.Card<Replicate>()
    ];

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        GD.Print("[Downfall] GenerateAnimator called");

        var animState = new AnimState("idle", true);
        var state1 = new AnimState("idle");
        var state2 = new AnimState("idle");
        var state3 = new AnimState("idle");
        var state4 = new AnimState("idle");
        var state5 = new AnimState("idle");
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