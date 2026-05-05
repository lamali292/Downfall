using Downfall.DownfallCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Snecko.SneckoCode.Events;

public static class SneckoHook
{
    public static Task AfterCardMuddled(ICombatState cs, PlayerChoiceContext ctx, CardModel card, AbstractModel? source)
    {
        return DownfallHook.Dispatch<IAfterCardMuddled>(cs, ctx, m => m.AfterCardMuddled(ctx, card, source));
    }

}