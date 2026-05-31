using Downfall.DownfallCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Events;

public static class SlimeBossHook
{
    public static Task AfterConsumeEffect(ICombatState cs, PlayerChoiceContext ctx, Creature creature,
        Creature attacker, int amount)
    {
        return DownfallHook.Dispatch<IAfterConsumeEffect>(cs,
            e => e.AfterConsumeEffect(ctx, creature, attacker, amount));
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


    public static int ModifySecondarySlimeEffects(ICombatState cs, int originalAmount,
        out IEnumerable<IModifySecondarySlimeEffects> modifiers, SlimeModel slime)
    {
        return DownfallHook.Modify(cs, originalAmount, (e, a) => e.ModifySecondarySlimeEffects(a, slime),
            out modifiers);
    }

    public static Task AfterSplit(ICombatState cs, Player player, SlimeModel slime)
    {
        return DownfallHook.Dispatch<IAfterSplit>(cs,
            e => e.AfterSplit(player, slime));
    }

    public static int ModifyConsumeCount(ICombatState cs, Player player, int amount, CardModel? cardSource,
        out IEnumerable<IModifyConsumeCount> modifiers)
    {
        return DownfallHook.Modify(cs, amount, (e, a) => e.ModifyConsumeCount(player, a, cardSource),
            out modifiers);
    }

    public static Task AfterModifyingConsumeCount(ICombatState cs, IEnumerable<IModifyConsumeCount> modifiers,
        Player player, CardModel? cardSource)
    {
        return DownfallHook.AfterModifying(cs, modifiers, e => e.AfterModifyingConsumeCount(player, cardSource));
    }
}