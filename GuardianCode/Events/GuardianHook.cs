using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Events;

using Alias = IAfterGemPlayed;

public static class GuardianHook
{
    public static Task AfterGuardianModeChange(ICombatState cs, PlayerChoiceContext ctx, Player player,
        GuardianModeModel oldMode,
        GuardianModeModel newMode)
    {
        return HookUtils.Dispatch<IAfterGuardianModeChange>(cs,
            m => m.AfterGuardianModeChange(ctx, player, oldMode, newMode));
    }

    public static Task AfterGuardianModeChangeEarly(ICombatState cs, PlayerChoiceContext ctx, Player player,
        GuardianModeModel oldMode,
        GuardianModeModel newMode)
    {
        return HookUtils.Dispatch<IAfterGuardianModeChangeEarly>(cs,
            m => m.AfterGuardianModeChangeEarly(ctx, player, oldMode, newMode));
    }


    public static Task BeforeCardEntersStasis(ICombatState cs, PlayerChoiceContext ctx, CardModel card,
        AbstractModel source)
    {
        return HookUtils.Dispatch<IBeforeCardEntersStasis>(cs, ctx,
            m => m.BeforeCardEntersStasis(ctx, card, source));
    }

    public static Task AfterCardEntersStasis(ICombatState cs, PlayerChoiceContext ctx, CardModel card,
        AbstractModel source)
    {
        return HookUtils.Dispatch<IAfterCardEntersStasis>(cs, ctx,
            m => m.AfterCardEntersStasis(ctx, card, source));
    }


    public static decimal ModifyGemEffect(ICombatState cs, GemModel gem, decimal baseValue, CardModel? card)
    {
        return HookUtils.Aggregate<IModifyGemEffect, decimal>(cs, baseValue,
            (m, val) => m.ModifyGemEffect(gem, val, card));
    }

    public static Task AfterGemPlayed(ICombatState? cs, PlayerChoiceContext ctx, GemModel gemModel, CardPlay? cardPlay)
    {
        return HookUtils.Dispatch<Alias>(cs, ctx,
            m => m.AfterGemPlayed(ctx, gemModel, cardPlay));
    }

    public static Task AfterCardTick(ICombatState cs, PlayerChoiceContext ctx, CardModel card, Player player)
    {
        return HookUtils.Dispatch<IAfterCardTick>(cs, ctx,
            m => m.AfterCardTick(ctx, card, player));
    }

    public static decimal ModifyBraceAmount(ICombatState cs, Player player, decimal amount,
        out IEnumerable<IModifyBraceAmount> modifiers)
    {
        return HookUtils.Modify(cs, amount,
            (m, val) => m.ModifyBraceAmount(player, val), out modifiers);
    }

    public static Task AfterModifyingBraceAmount(ICombatState cs, Player player, decimal modifiedAmount,
        IEnumerable<IModifyBraceAmount> modifiers)
    {
        return HookUtils.AfterModifying(cs, modifiers,
            m => m.AfterModifyingBraceAmount(player, modifiedAmount));
    }

    public static Task AfterBrace(ICombatState cs, Player player, decimal amount)
    {
        return HookUtils.Dispatch<IAfterBrace>(cs, m => m.AfterBrace(player, amount));
    }
}