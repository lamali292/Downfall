using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Snecko.SneckoCode.Events;

public static class SneckoHook
{

    public static Task AfterOverflowEffect(Player player, CardPlay cardPlay, CardModel card)
    {
        return HookUtils.DispatchWithContext<IAfterOverflowEffect>(player,
            (m, ctx) => m.AfterOverflowEffect(ctx, cardPlay, card));
    }
}