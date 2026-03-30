using Downfall.Code.Abstract;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Powers.Awakened;

public class ArchmagusPower : AwakenedPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        return card.Owner.Creature != Owner || card is not ISpell || 
               CombatManager.Instance.History.CardPlaysStarted.Count(e => 
                   e.Actor == Owner && 
                   e.CardPlay is { IsFirstInSeries: true, Card: ISpell } &&
                   e.HappenedThisTurn(CombatState)) >= Amount ? playCount : playCount + 1;
    }

    public override Task AfterModifyingCardPlayCount(CardModel card)
    {
        Flash();
        return Task.CompletedTask;
    }
}