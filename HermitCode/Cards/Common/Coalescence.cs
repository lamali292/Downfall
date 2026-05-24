using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Common;

public sealed class Coalescence : HermitCardModel
{
    public Coalescence() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(6, 3);
        WithCards(2, 1);
        WithTip(CardKeyword.Retain);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        var hand = PileType.Hand.GetPile(Owner);
        if (hand.Cards.Count == 0) return;

        var retainable = hand.Cards.Where(c => !c.ShouldRetainThisTurn).ToList();
        if (retainable.Count == 0) return;

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, DynamicVars.Cards.IntValue);
        var selected = (await CardSelectCmd.FromHand(
            prefs: prefs,
            context: ctx,
            player: Owner,
            filter: c => !c.ShouldRetainThisTurn,
            source: this
        )).ToList();

        foreach (var card in selected) card.GiveSingleTurnRetain();
    }
}