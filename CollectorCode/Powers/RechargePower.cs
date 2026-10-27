using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Powers;

public class RechargePower : CollectorPowerModel
{
    public RechargePower()
    {
        WithReserveTip();
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            if (CombatManager.Instance.History.CardPlaysFinished.Count(e =>
                    e.Actor == Owner && e.HappenedThisTurn(CombatState) && e.CardPlay.Card.Type == CardType.Attack) == 0)
            {
                Flash();
                await PowerCmd.Apply<ReserveNextTurnPower>(choiceContext, Owner, Amount, Owner, null);
            }
        }
    }
}