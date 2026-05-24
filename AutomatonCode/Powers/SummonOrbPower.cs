using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Powers;

public class SummonOrbPower : AutomatonPowerModel
{
 
    // TODO : don't stash Summon Orb itself
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || !cardPlay.IsFirstInSeries) return;
        var playedThisTurn = CombatManager.Instance.History.CardPlaysStarted
            .Count(e => e.Actor == Owner && e.CardPlay.IsFirstInSeries && e.HappenedThisTurn(CombatState));
        
        if (playedThisTurn > Amount) return;
        await StashCmd.Stash(cardPlay.Card);
    }
}