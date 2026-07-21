using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Events;

/// <summary>
///     Provides utility methods for dispatching and aggregating combat hook events
///     across all <see cref="ICombatState" /> hook listeners.
///     <para />
///     Hook interfaces should be implemented on <see cref="AbstractModel" /> subclasses
///     to be picked up by the listeners.
/// </summary>
public static class DownfallHook
{
  

    public static Task AfterCustomDraw(ICombatState cs, PlayerChoiceContext ctx, Player player, PileType pile,
        CardPileAddResult result)
    {
        return HookUtils.Dispatch<IAfterCustomDraw>(cs, ctx, m => m.AfterCustomDraw(player, pile, result));
    }

    public static Task AfterSoulburnDetonate(ICombatState cs, PlayerChoiceContext ctx, Creature creature)
    {
        return HookUtils.Dispatch<IAfterSoulburnDetonate>(cs, ctx, m => m.AfterSoulburnDetonate(ctx, creature));
    }

    public static Task<bool> ShouldSoulburnDetonateTargetAll(ICombatState cs, PlayerChoiceContext ctx, Creature owner)
    {
        return Task.FromResult(
            HookUtils.Any<IShouldSoulburnDetonateTargetAll>(cs, m => m.ShouldSoulburnDetonateTargetAll(ctx, owner)));
    }

    public static decimal ModifySelfDamage(ICombatState cs, decimal original, AbstractModel model,
        out IEnumerable<IModifySelfDamage> modifiers)
    {
        return HookUtils.Modify(cs, original, (m, a) => m.ModifySelfDamage(a, model), out modifiers);
    }

    public static Task AfterModifyingSelfDamage(ICombatState cs, IEnumerable<IModifySelfDamage> modifiers,
        AbstractModel model)
    {
        return HookUtils.AfterModifying(cs, modifiers, m => m.AfterModifyingSelfDamage(model));
    }
}