using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Powers;

public class SummonOrbPower : AutomatonPowerModel
{
    // TODO : maybe try to code similar to NostalgiaPower. but i had issues previously with this
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || !cardPlay.IsFirstInSeries || cardPlay.Card.Type is not (CardType.Attack or CardType.Skill)) return;
        var playedThisTurn = CombatManager.Instance.History.CardPlaysStarted
            .Count(e => e.Actor == Owner && e.CardPlay.Card.Type is CardType.Attack or CardType.Skill && e.CardPlay.IsFirstInSeries && e.HappenedThisTurn(CombatState));

        if (playedThisTurn > Amount) return;
        await StashCmd.Stash(cardPlay.Card);
        Flash();
    }
}