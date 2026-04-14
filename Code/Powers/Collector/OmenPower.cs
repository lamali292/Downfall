using Downfall.Code.Abstract;
using Downfall.Code.Cards.Collector.Token;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Powers.Collector;

public class OmenPower : CollectorPowerModel
{
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || cardPlay.Card is not ICollectible) return;
        await PowerCmd.Apply<StrengthPower>(Owner, Amount, Owner, null);
    }
}