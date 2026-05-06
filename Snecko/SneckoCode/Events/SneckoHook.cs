using Downfall.DownfallCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Cards;

namespace Snecko.SneckoCode.Events;

public static class SneckoHook
{
    public static Task AfterCardMuddled(ICombatState cs, PlayerChoiceContext ctx, CardModel card, AbstractModel? source)
    {
        return DownfallHook.Dispatch<IAfterCardMuddled>(cs, ctx, m => m.AfterCardMuddled(ctx, card, source));
    }

    public static Task AfterOverflowEffect(ICombatState cs, PlayerChoiceContext ctx, CardPlay cardPlay, CardModel card)
    {
        return DownfallHook.Dispatch<IAfterOverflowEffect>(cs, ctx,  m => m.AfterOverflowEffect(ctx, cardPlay, card));
    }
}