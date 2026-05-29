using Collector.CollectorCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Core;

public static class CollectorCardEffectHandler
{
    public static async Task<bool> DoBeforeOnPlay(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (card is not IHasPyre pyre) return true;
        pyre.PyredCard = await CollectorCmd.Pyre(ctx, card);
        return pyre.PyredCard != null;
    }
}