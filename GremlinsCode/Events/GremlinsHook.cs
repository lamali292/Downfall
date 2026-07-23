using BaseLib.Utils;
using Downfall.DownfallCode.Events;
using Gremlins.GremlinsCode.Core;
using Gremlins.GremlinsCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Gremlins.GremlinsCode.Events;

public class GremlinsHook
{
    public static Task AfterGremlinSwap(ICombatState cs, PlayerChoiceContext ctx, Player player,
        GremlinSwapType gremlinSwapType)
    {
        return HookUtils.Dispatch<IAfterGremlinSwap>(cs, ctx,
            m => m.AfterGremlinSwap(ctx, player, gremlinSwapType));
    }


    public static decimal ModifyWizExtraDamage(WizPower wizPower, int extraDamage)
    {
        return HookUtils.Aggregate<IModifyWizExtraDamage, decimal>(wizPower.CombatState,
            extraDamage, (m, k) => m.ModifyWizExtraDamage(wizPower, k));
    }

    public static bool ShouldGremlinSwap(ICombatState cs, Player player, Creature gremlin)
    {
        return HookUtils.All<IShouldGremlinSwap>(cs, m => m.ShouldGremlinSwap(player, gremlin));
    }

    public static Task AfterWizConsumed(ICombatState cs, PlayerChoiceContext ctx, Creature oldOwner)
    {
        return HookUtils.Dispatch<IAfterWizConsumed>(cs, ctx,
            m => m.AfterWizConsumed(ctx, oldOwner));
    }
}