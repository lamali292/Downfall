using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Events;

public static class CollectorHook
{
    public static int ModifyCollectorMiasmaIncrement(ICombatState cs, Creature creature, int baseAmount)
    {
        return HookUtils.Aggregate<IModifyCollectorMiasmaIncrement, int>(cs, baseAmount,
            (m, current) => m.ModifyCollectorMiasmaIncrement(creature, current));
    }

    public static Task AfterCardPyred(ICombatState cs, PlayerChoiceContext ctx, CardModel card, CardModel pyred)
    {
        return HookUtils.Dispatch<IAfterCardPyred>(cs, ctx, m => m.AfterCardPyred(ctx, card, pyred));
    }

    public static bool ShouldExhaustPyred(CardModel card, CardModel pyred)
    {
        return HookUtils.All<IShouldExhaustPyred>(card.CombatState!, e => e.ShouldExhaustPyred(card, pyred));
    }

    public static bool ShouldTorchheadTargetAll(Player player, out IEnumerable<IShouldTorchheadTargetAll> modifiers)
    {
        if (player.Creature.CombatState != null)
            return HookUtils.Any(player.Creature.CombatState!, e => e.ShouldTorchheadTargetAll(player), out modifiers);
        modifiers = [];
        return false;
    }
    
    public static Task AfterShouldTorchheadTargetAll(PlayerChoiceContext ctx, Player player, IEnumerable<IShouldTorchheadTargetAll> modifiers)
    {
        if (player.Creature.CombatState == null) return Task.CompletedTask;
        return HookUtils.AfterModifying(player.Creature.CombatState, modifiers, e => e.AfterShouldTorchheadTargetAll(ctx, player));
    }
}

