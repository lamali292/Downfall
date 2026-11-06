using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using Champ.ChampCode.History;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Powers;

public class DancingMasterPower : ChampPowerModel, IOnFinisher
{
    public async Task OnFinisher(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player || !FinisherEntry.IsFirstThisTurn(cardPlay.Card.Owner)) return;

        await PlayerCmd.GainEnergy(Amount, cardPlay.Card.Owner);
        await CardPileCmd.Draw(ctx, Amount, cardPlay.Card.Owner);
        Flash();
    }
}