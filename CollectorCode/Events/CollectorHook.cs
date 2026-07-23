using BaseLib.Utils;
using Downfall.DownfallCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Events;

public static class CollectorHook
{
    public static int ModifyCollectorDoomDamage(ICombatState cs, Creature creature, int baseAmount)
    {
        return HookUtils.Aggregate<IModifyCollectorDoomDamage, int>(cs, baseAmount,
            (m, current) => m.ModifyCollectorDoomDamage(creature, current));
    }

    public static bool PreventDoomRemoval(ICombatState cs, Creature creature)
    {
        return HookUtils.Any<IPreventDoomRemoval>(cs, m => m.PreventDoomRemoval(creature));
    }

    public static bool PreventCollectedDraw(ICombatState cs, Player player)
    {
        return HookUtils.Any<IPreventCollectedDraw>(cs, m => m.PreventCollectedDraw(player));
    }

    public static Task OnPyre(ICombatState cs, PlayerChoiceContext ctx, CardModel card, CardModel pyred)
    {
        return HookUtils.Dispatch<IOnPyre>(cs, ctx, m => m.OnPyre(ctx, card, pyred));
    }
}