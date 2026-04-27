using Downfall.DownfallCode.Events;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Events;

public static class HexaghostHook
{
    

    public static int ModifyGhostflameEffectAdditive(ICombatState cs, Player owner,
        GhostflameModel ghostflameModel)
    {
        return DownfallHook.Aggregate<IModifyGhostflameEffectAdditive, int>(cs, 0,
            (m, current) => current + m.ModifyGhostflameEffectAdditive(owner, ghostflameModel));
    }
    
    public static int ModifyGhostflameRepeatAdditive(ICombatState cs, Player owner, GhostflameRepeatType repeatType, GhostflameModel ghostflameModel)
    {
        return DownfallHook.Aggregate<IModifyGhostflameRepeatAdditive, int>(cs, 0,
            (m, current) => current + m.ModifyGhostflameRepeatAdditive(owner, repeatType, ghostflameModel));
    }

    public static Task AfterWheelRetract(ICombatState cs, PlayerChoiceContext ctx, Player player, CardModel? source,
        GhostflameModel ghostflame, int ghostflameIndex, bool silent)
    {
        return DownfallHook.Dispatch<IWheelMoved>(cs, ctx,
            m => m.AfterWheelRetract(ctx, player, source, ghostflame, ghostflameIndex, silent));
    }

    public static Task AfterWheelAdvance(ICombatState cs, PlayerChoiceContext ctx, Player player, CardModel? source,
        GhostflameModel ghostflame, int ghostflameIndex, bool silent)
    {
        return DownfallHook.Dispatch<IWheelMoved>(cs, ctx,
            m => m.AfterWheelAdvance(ctx, player, source, ghostflame, ghostflameIndex, silent));
    }

    public static Task AfterSoulburnDetonate(ICombatState cs, PlayerChoiceContext ctx, Creature creature)
    {
        return DownfallHook.Dispatch<IAfterSoulburnDetonate>(cs, ctx, m => m.AfterSoulburnDetonate(ctx, creature));
    }

    public static Task<bool> ShouldSoulburnDetonateTargetAll(ICombatState cs, PlayerChoiceContext ctx, Creature owner)
    {
        return Task.FromResult(DownfallHook.Any<IShouldSoulburnDetonateTargetAll>(cs, m => m.ShouldSoulburnDetonateTargetAll(ctx, owner)));
    }

    public static Task AfterGhostwheelIgnited(ICombatState cs, PlayerChoiceContext ctx, Player player, GhostflameModel flame, int index)
    {
        return DownfallHook.Dispatch<IAfterGhostwheelIgnited>(cs, ctx, m => m.AfterGhostwheelIgnited(ctx, player, flame, index));
    }

    public static Task AfterGhostwheelAllIgnited(ICombatState cs, PlayerChoiceContext ctx, Player player, GhostflameModel flame, int index)
    {
        return DownfallHook.Dispatch<IAfterGhostwheelAllIgnited>(cs, ctx, m => m.AfterGhostwheelAllIgnited(ctx, player, flame, index));
    }

 
}