using Downfall.DownfallCode.Events;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Events;

using Alias = IAfterGemPlayed;

public static class GuardianHook
{
    public static Task OnGuardianModeChange(ICombatState cs, PlayerChoiceContext ctx, Player player,
        GuardianModeModel oldMode,
        GuardianModeModel newMode)
    {
        return DownfallHook.Dispatch<IOnGuardianModeChange>(cs,
            m => m.OnGuardianModeChange(ctx, player, oldMode, newMode));
    }


    public static Task BeforeCardEntersStasis(ICombatState cs, PlayerChoiceContext ctx, CardModel card,
        AbstractModel source)
    {
        return DownfallHook.Dispatch<IBeforeCardEntersStasis>(cs, ctx,
            m => m.BeforeCardEntersStasis(ctx, card, source));
    }


    public static decimal ModifyGemEffect(ICombatState cs, GemModel gem, decimal baseValue, CardModel card)
    {
        return DownfallHook.Aggregate<IModifyGemEffect, decimal>(cs, baseValue,
            (m, val) => m.ModifyGemEffect(gem, val, card));
    }

    public static Task AfterGemPlayed(ICombatState cs, PlayerChoiceContext ctx, GemModel gemModel, CardPlay? cardPlay)
    {
        return DownfallHook.Dispatch<Alias>(cs, ctx,
            m => m.AfterGemPlayed(ctx, gemModel, cardPlay));
    }
}