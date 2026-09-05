using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Powers;

public class AshesToAshesPower : CollectorPowerModel
{
    public AshesToAshesPower()
    {
        WithTip<StrengthPower>();
        WithTip(CardKeyword.Exhaust);
    }
    
    public override async Task AfterCardExhausted(
        PlayerChoiceContext ctx,
        CardModel card,
        bool _)
    {
        if (card.Owner.Creature != Owner)
            return;
        await PowerCmd.Apply<StrengthPower>(ctx, Owner, Amount, Owner, null);
        Flash();
    }
}