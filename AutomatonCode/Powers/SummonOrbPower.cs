using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Powers;

public class SummonOrbPower : AutomatonPowerModel
{
    public override int DisplayAmount => Math.Max(Amount - PlayedThisTurn, 0);

    private int PlayedThisTurn => CombatManager.Instance.History.CardPlaysStarted
        .Count(e => e.Actor == Owner && IsCardWeWant(e.CardPlay) && e.HappenedThisTurn(CombatState));

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return Task.CompletedTask;
        this.InvokeSilentDisplayAmountChanged();
        return Task.CompletedTask;
    }

    // TODO : maybe try to code similar to NostalgiaPower. but i had issues previously with this
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (Owner != cardPlay.Card.Owner.Creature) return;
        if (!IsCardWeWant(cardPlay)) return;
        if (PlayedThisTurn > Amount) return;
        InvokeDisplayAmountChanged();
        if (StashCmd.IsFull(cardPlay.Card.Owner)) { return;}
        await StashCmd.Stash(ctx, cardPlay.Card);
        //Flash();
    }

    private bool IsCardWeWant(CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        return cardPlay.IsFirstInSeries && 
               card.Type is CardType.Attack or CardType.Skill && 
               !card.Keywords.Contains(CardKeyword.Exhaust) &&
               !AutomatonCmd.IsEncodable(card);
    }
}