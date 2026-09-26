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

    public static int ModifySecondarySlimeEffects(ICombatState cs, int originalAmount,
        out IEnumerable<IModifySecondarySlimeEffects> modifiers, SlimeModel slime)
    {
        return HookUtils.Modify(cs, originalAmount, (e, a) => e.ModifySecondarySlimeEffects(a, slime),
            out modifiers);
    }

    public static Task AfterSplit(ICombatState cs, PlayerChoiceContext ctx, Player player, SlimeModel slime)
    {
        return HookUtils.Dispatch<IAfterSplit>(cs, ctx, 
            e => e.AfterSplit(ctx, player, slime));
    }

    public static Task AfterCommand(ICombatState cs, PlayerChoiceContext ctx, Player player, SlimeModel slime, CardModel? source)
    {
        return HookUtils.Dispatch<IAfterCommand>(cs, ctx, 
            e => e.AfterCommand(ctx, player, slime, source));
    }
}