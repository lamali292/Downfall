using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Powers;

public class GremlinAxePower : CollectorPowerModel
{
    public GremlinAxePower()
    {
        WithTip<VigorPower>();
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player || cardPlay.Card.Type != CardType.Skill )
            return;
        Flash();
        await PowerCmd.Apply<VigorPower>(ctx, Owner, Amount, Owner, null);
    }
}