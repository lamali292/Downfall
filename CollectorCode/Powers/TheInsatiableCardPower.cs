using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Events;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Powers;

public class TheInsatiableCardPower : CollectorPowerModel, IAfterCardPyred
{
    public TheInsatiableCardPower()
    {
        WithTip(CollectorTip.Kindle);
        WithTip(CollectorTip.Pyred);
        WithTip(CollectorKeyword.Pyre);
        WithTip(CardKeyword.Exhaust);
        WithEnergy(2);
    }
    
    public async Task AfterCardPyred(PlayerChoiceContext ctx, CardModel card, CardModel pyred)
    {
        if (pyred.Owner.Creature != Owner) return;
        if (pyred.EnergyCost.GetAmountToSpend() < DynamicVars.Energy.IntValue) return;
        await CollectorCmd.Kindle(ctx, pyred.Owner, Amount, this);
        Flash();
    }
}