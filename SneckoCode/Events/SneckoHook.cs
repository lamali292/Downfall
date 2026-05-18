using Downfall.DownfallCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Snecko.SneckoCode.Events;

public static class SneckoHook
{
    public static Task AfterCardMuddled(ICombatState cs, PlayerChoiceContext ctx, CardModel card, AbstractModel? source)
    {
        return DownfallHook.Dispatch<IAfterCardMuddled>(cs, ctx, m => m.AfterCardMuddled(ctx, card, source));
    }

    public static Task AfterOverflowEffect(ICombatState cs, CardPlay cardPlay, CardModel card)
    {
        return DownfallHook.DispatchWithContext<IAfterOverflowEffect>(cs,
            (m, ctx) => m.AfterOverflowEffect(ctx, cardPlay, card));
    }

    public static bool ShouldAllowMuddleCost(ICombatState cs, CardModel card, int cost)
    {
        return DownfallHook.All<IShouldAllowMuddleCost>(cs, m => m.ShouldAllowMuddleCost(card, cost));
    }
}