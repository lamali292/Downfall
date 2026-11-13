using BaseLib.Utils;
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
        Creature attacker)
    {
        return MyHookUtils.Dispatch<IAfterConsumeEffect>(cs,
            e => e.AfterConsumeEffect(ctx, creature, attacker), MyHookUtils.HookScope.CombatRaw);
    }

    public static bool TryPreventConsumeWeakRemoval(ICombatState cs, Creature target, Creature consumer)
    {
        return MyHookUtils.Any<IPreventConsumeWeakRemoval>(cs, e => e.TryPreventWeakRemoval(target, consumer));
    }

    /// <summary>
    /// A Slime's non-damage effect after modifiers: <c>(base + additive) * multiplicative</c>. Like damage,
    /// every additive modifier applies before any multiplicative one, whatever order the powers were applied in.
    /// </summary>
    public static int ModifySlimeEffects(ICombatState cs, int originalAmount, out IEnumerable<IModifySlimeEffects> modifiers,
        SlimeModel slime)
    {
        var added = HookUtils.Modify<IModifySlimeEffects, int>(cs, originalAmount, (e, a) => a + e.ModifySlimeEffectsAdditive(slime),
            out var additive);
        var multiplied = HookUtils.Modify<IModifySlimeEffects, int>(cs, added, (e, a) => a * e.ModifySlimeEffectsMultiplicative(slime),
            out var multiplicative);
        modifiers = additive.Union(multiplicative);
        return multiplied;
    }

    public static Task AfterSplit(ICombatState cs, PlayerChoiceContext ctx, Player player, SlimeModel slime)
    {
        return HookUtils.Dispatch<IAfterSplit>(cs, ctx, 
            e => e.AfterSplit(ctx, player, slime));
    }

    public static Task AfterCommand(ICombatState cs, PlayerChoiceContext ctx, Player player, SlimeModel slime,
        CardModel? source, bool isAutomatic)
    {
        return HookUtils.Dispatch<IAfterCommand>(cs, ctx,
            e => e.AfterCommand(ctx, player, slime, source, isAutomatic));
    }
}