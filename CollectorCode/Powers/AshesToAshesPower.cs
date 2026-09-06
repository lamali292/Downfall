using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Powers;

public class AshesToAshesPower : CollectorPowerModel, IAfterCardPyred
{
    public AshesToAshesPower()
    {
        WithTip<StrengthPower>();
        WithTip(CollectorKeyword.Pyre);
        WithTip(CollectorTip.Pyred);
        WithTip(CardKeyword.Exhaust);
    }
    
    public async Task AfterCardPyred(PlayerChoiceContext ctx, CardModel card, CardModel pyred)
    {
        if (pyred.Owner.Creature != Owner)
            return;
        await PowerCmd.Apply<StrengthPower>(ctx, Owner, Amount, Owner, null);
        Flash();
    }
}