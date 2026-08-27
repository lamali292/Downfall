using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Powers;

public class ForkedTonguePower : SneckoPowerModel
{
    private const int CostThreshold = 3;

    private int PlayedThisTurn => CombatManager.Instance.History.CardPlaysStarted.Count(e =>
        e.Actor == Owner && e.CardPlay.IsFirstInSeries && e.HappenedThisTurn(CombatState) &&
        IsHighCost(e.CardPlay.Card)
    );

    private static bool IsHighCost(CardModel card)
    {
        return card.EnergyCost.GetResolved() >= CostThreshold;
    }


    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        return card.Owner.Creature != Owner || !IsHighCost(card) ||
               PlayedThisTurn >= Amount
            ? playCount
            : playCount + 1;
    }

    public override Task AfterModifyingCardPlayCount(CardModel card)
    {
        Flash();
        return Task.CompletedTask;
    }
}