using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Powers;

public class BrainDrainPower : CollectorPowerModel
{
    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != Owner || !card.VisualCardPool.IsColorless)
            return false;
        if (card.Pile?.Type is not (PileType.Hand or PileType.Play))
            return false;
        modifiedCost = 0M;
        return true;
    }
 
    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.Owner.Creature != Owner ||!card.VisualCardPool.IsColorless)
            return;
        if (card.Pile?.Type is not (PileType.Hand or PileType.Play))
            return;
        await PowerCmd.Decrement(this);
    }

}