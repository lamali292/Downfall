using BaseLib.Abstracts;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Powers;

public class InfestedPrismCardPower : CollectorPowerModel
{
    public InfestedPrismCardPower()
    {
        WithTip<StrengthPower>();
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner ||cardPlay.Card.Type != CardType.Skill) return;
        await PowerCmd.Apply<InfestedPrismCardPowerPower>(ctx, Owner, Amount, Owner, null);
    }
}

public class InfestedPrismCardPowerPower : CustomTemporaryPowerModelWrapper<InfestedPrismCardPower, StrengthPower>;