using Automaton.AutomatonCode.Core;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Powers;

public class SummonOrbPower : AutomatonPowerModel
{
    // TODO : maybe try to code similar to NostalgiaPower. but i had issues previously with this
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;
        if (!GuardianCmd.CanPutIntoStasis(player)) return;
        if (!IsCardWeWant(cardPlay)) return;
       var playedThisTurn = CombatManager.Instance.History.CardPlaysStarted
            .Count(e => e.Actor == Owner && IsCardWeWant(e.CardPlay) && e.HappenedThisTurn(CombatState));

        if (playedThisTurn > Amount) return;
        await StashCmd.Stash(ctx, cardPlay.Card);
        Flash();
    }

    private bool IsCardWeWant(CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        var player = card.Owner;
        return player.Creature == Owner && 
               cardPlay.IsFirstInSeries && 
               card.Type is CardType.Attack or CardType.Skill && 
               !card.Keywords.Contains(CardKeyword.Exhaust) &&
               !AutomatonCmd.IsEncodable(card);
    }
}