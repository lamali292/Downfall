using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Powers;

public class CeremonialBeastCardPower() : CollectorPowerModel(PowerType.Debuff)
{
    private CardModel? _source;
    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        _source = cardSource;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner) return;
        if (cardPlay.Card == _source)
        {
            _source = null;
            return;
        }
        if (Amount <= 1)
        {
            PlayerCmd.EndTurn(cardPlay.Card.Owner, false);
        }
        await PowerCmd.Decrement(this);
    }
    
    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;
        await PowerCmd.Remove(this);
    }
}