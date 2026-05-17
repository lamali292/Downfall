using Downfall.DownfallCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SlimeBoss.SlimeBossCode.Events;

public static class SlimeBossHook
{
    public static Task AfterConsumeEffect(ICombatState cs, PlayerChoiceContext ctx, Creature creature,
        AttackCommand command, int amount)
    {
        return DownfallHook.Dispatch<IAfterConsumeEffect>(cs,
            e => e.AfterConsumeEffect(ctx, creature, command, amount));
    }

    public static int ModifyGoopConsume(ICombatState cs, int originalAmount,
        out IEnumerable<IModifyGoopConsume> modifiers, Creature creature, Creature? applier)
    {
        return DownfallHook.Modify(cs, originalAmount, (e, a) => e.ModifyGoopConsume(a, creature, applier),
            out modifiers);
    }

    public static Task AfterModifyingGoopConsume(ICombatState cs, IEnumerable<IModifyGoopConsume> modifiers,
        Creature creature, Creature? applier)
    {
        return DownfallHook.AfterModifying(cs, modifiers, e => e.AfterModifyingGoopConsume(creature, applier));
    }
}