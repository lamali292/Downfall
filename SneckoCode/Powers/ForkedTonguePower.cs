using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Powers;

public class ForkedTonguePower : SneckoPowerModel, IHasSecondAmount
{
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        return card.Owner.Creature != Owner || !SneckoCmd.IsOffclass(card) ||
               PlayedThisTurn >= Amount ? 
            playCount : playCount + 1;
    }

    private int PlayedThisTurn => CombatManager.Instance.History.CardPlaysStarted.Count(e =>
        e.Actor == Owner && e.CardPlay.IsFirstInSeries && e.HappenedThisTurn(CombatState) && SneckoCmd.IsOffclass(e.CardPlay.Card)
    );
    
    public override int DisplayAmount => Math.Max(0, Amount - PlayedThisTurn);
    
    public override Task AfterModifyingCardPlayCount(CardModel card)
    {
        Flash();
        return Task.CompletedTask;
    }

    public override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return Task.CompletedTask;
        this.InvokeSecondAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner) return Task.CompletedTask;
        this.InvokeSecondAmountChanged();
        return Task.CompletedTask;
    }

    public string GetSecondAmount()
    {
        return "";
    }
}