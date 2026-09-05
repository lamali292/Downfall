using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Core;

public static class CollectorCardEffectHandler
{
    public static async Task<bool> DoBeforeOnPlayInternal(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (!card.Keywords.Contains(CollectorKeyword.Pyre)) return true;
        var pyred = await CollectorCmd.Pyre(ctx, card);
        if (card is IUsesPyredCard pyre) pyre.PyredCard = pyred;
        return pyred != null;
    }
}