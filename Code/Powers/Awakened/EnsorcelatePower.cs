using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Powers.Awakened;

public class EnsorcelatePower : AwakenedPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;


    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (!IsEligiblePower(card)) return false;
        modifiedCost = 0M;
        return true;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (IsEligiblePower(cardPlay.Card)) await PowerCmd.Decrement(this);
    }

    private bool IsEligiblePower(CardModel card)
    {
        return card.Type == CardType.Power &&
               card.Owner.Creature == Owner &&
               card.Pile?.Type is PileType.Hand or PileType.Play;
    }
}