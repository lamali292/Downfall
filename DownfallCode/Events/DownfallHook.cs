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
    /// <summary>
    ///     Dispatches an action to all hook listeners of type <typeparamref name="THook" />.
    /// </summary>
    /// <typeparam name="THook">The hook interface to filter listeners by.</typeparam>
    /// <param name="combatState">The current combat state to iterate listeners from.</param>
    /// <param name="action">The async action to invoke on each matching listener.</param>
    public static async Task Dispatch<THook>(ICombatState combatState, Func<THook, Task> action)
        where THook : class
    {
        foreach (var model in combatState.IterateHookListeners().OfType<THook>())
            await action(model);
    }

    /// <summary>
    ///     Dispatches an action to all hook listeners of type <typeparamref name="THook" />,
    ///     pushing and popping each listener onto the provided <see cref="PlayerChoiceContext" />.
    ///     Silently skips listeners that are not <see cref="AbstractModel" /> instances.
    /// </summary>
    /// <typeparam name="THook">The hook interface to filter listeners by.</typeparam>
    /// <param name="combatState">The current combat state to iterate listeners from.</param>
    /// <param name="ctx">The player choice context to push/pop each model onto.</param>
    /// <param name="action">The async action to invoke on each matching listener.</param>
    public static async Task Dispatch<THook>(ICombatState combatState, PlayerChoiceContext ctx,
        Func<THook, Task> action)
        where THook : class
    {
        foreach (var model in combatState.IterateHookListeners().OfType<THook>())
        {
            if (model is not AbstractModel abstractModel) continue;
            ctx.PushModel(abstractModel);
            await action(model);
            ctx.PopModel(abstractModel);
        }
    }

    /// <summary>
    ///     Dispatches an action to all hook listeners of type <typeparamref name="THook" />,
    ///     creating a <see cref="HookPlayerChoiceContext" /> for each listener and awaiting
    ///     its completion or pause. Silently skips listeners that are not <see cref="AbstractModel" /> instances.
    /// </summary>
    /// <typeparam name="THook">The hook interface to filter listeners by.</typeparam>
    /// <param name="combatState">The current combat state to iterate listeners from.</param>
    /// <param name="action">
    ///     The async action to invoke on each matching listener, receiving a
    ///     <see cref="PlayerChoiceContext" />.
    /// </param>
    public static async Task DispatchWithContext<THook>(ICombatState combatState,
        Func<THook, PlayerChoiceContext, Task> action)
        where THook : class
    {
        var netId = LocalContext.NetId;
        if (!netId.HasValue) return;
        foreach (var model in combatState.IterateHookListeners().OfType<THook>())
        {
            if (model is not AbstractModel abstractModel) continue;
            var hookCtx = new HookPlayerChoiceContext(abstractModel, netId.Value, combatState, GameActionType.Combat);
            var task = action(model, hookCtx);
            await hookCtx.AssignTaskAndWaitForPauseOrCompletion(task);
            abstractModel.InvokeExecutionFinished();
        }
    }


    /// <summary>
    ///     Aggregates a value across all hook listeners of type <typeparamref name="THook" />,
    ///     passing each listener and the current accumulated value to the provided function.
    /// </summary>
    /// <typeparam name="THook">The hook interface to filter listeners by.</typeparam>
    /// <typeparam name="TResult">The type of the accumulated result.</typeparam>
    /// <param name="combatState">The current combat state to iterate listeners from.</param>
    /// <param name="initial">The initial value for the aggregation.</param>
    /// <param name="action">A function that takes a listener and the current value and returns the new value.</param>
    /// <returns>The final aggregated value after all listeners have been processed.</returns>
    public static TResult Aggregate<THook, TResult>(ICombatState combatState, TResult initial,
        Func<THook, TResult, TResult> action)
        where THook : class
    {
        return combatState.IterateHookListeners().OfType<THook>()
            .Aggregate(initial, (current, model) => action(model, current));
    }

    /// <summary>
    ///     Returns <see langword="true" /> if any hook listener of type <typeparamref name="THook" />
    ///     satisfies the given predicate.
    /// </summary>
    /// <typeparam name="THook">The hook interface to filter listeners by.</typeparam>
    /// <param name="combatState">The current combat state to iterate listeners from.</param>
    /// <param name="predicate">The condition to test each listener against.</param>
    public static bool Any<THook>(ICombatState combatState, Func<THook, bool> predicate)
        where THook : class
    {
        return combatState.IterateHookListeners().OfType<THook>().Any(predicate);
    }

    /// <summary>
    ///     Returns <see langword="true" /> if all hook listeners of type <typeparamref name="THook" />
    ///     satisfy the given predicate.
    /// </summary>
    /// <typeparam name="THook">The hook interface to filter listeners by.</typeparam>
    /// <param name="combatState">The current combat state to iterate listeners from.</param>
    /// <param name="predicate">The condition to test each listener against.</param>
    public static bool All<THook>(ICombatState combatState, Func<THook, bool> predicate)
        where THook : class
    {
        return combatState.IterateHookListeners().OfType<THook>().All(predicate);
    }

    public static bool Any<THook>(ICombatState combatState, Func<THook, bool> predicate, out IEnumerable<THook> matches)
        where THook : class
    {
        var list = combatState.IterateHookListeners().OfType<THook>().Where(predicate).ToList();
        matches = list;
        return list.Count > 0;
    }

    public static bool All<THook>(ICombatState combatState, Func<THook, bool> predicate,
        out IEnumerable<THook> nonMatches)
        where THook : class
    {
        var list = combatState.IterateHookListeners().OfType<THook>().Where(m => !predicate(m)).ToList();
        nonMatches = list;
        return list.Count == 0;
    }

    /// <summary>
    ///     Passes a value through all hook listeners of type <typeparamref name="THook" />,
    ///     tracking which listeners actually modified the value.
    /// </summary>
    /// <typeparam name="THook">The hook interface to filter listeners by.</typeparam>
    /// <typeparam name="TValue">The type of the value being modified. Must implement <see cref="IEquatable{T}" />.</typeparam>
    /// <param name="combatState">The current combat state to iterate listeners from.</param>
    /// <param name="originalAmount">The initial value before any modifications.</param>
    /// <param name="amountModifier">A function that takes a listener and the current value and returns the modified value.</param>
    /// <param name="modifiers">Outputs the set of listeners that actually changed the value.</param>
    /// <returns>The final modified value after all listeners have been processed.</returns>
    public static TValue Modify<THook, TValue>(
        ICombatState combatState,
        TValue originalAmount,
        Func<THook, TValue, TValue> amountModifier,
        out IEnumerable<THook> modifiers)
        where THook : class
        where TValue : IEquatable<TValue>
    {
        var amount = originalAmount;
        var abstractModelList = new List<THook>();
        foreach (var model in combatState.IterateHookListeners().OfType<THook>())
        {
            var previous = amount;
            amount = amountModifier.Invoke(model, amount);
            if (!previous.Equals(amount))
                abstractModelList.Add(model);
        }

        modifiers = abstractModelList;
        return amount;
    }

    /// <summary>
    ///     Invokes a follow-up action on all listeners that previously modified a value via
    ///     <see cref="Modify{THook,TValue}" />,
    ///     iterating in hook listener order and invoking <see cref="AbstractModel.InvokeExecutionFinished" /> for each.
    /// </summary>
    /// <typeparam name="THook">The hook interface to filter listeners by.</typeparam>
    /// <param name="cs">The current combat state to iterate listeners from.</param>
    /// <param name="modifiers">
    ///     The set of listeners that modified the value, as returned by
    ///     <see cref="Modify{THook,TValue}" />.
    /// </param>
    /// <param name="action">The async action to invoke on each modifier.</param>
    public static async Task AfterModifying<THook>(ICombatState cs, IEnumerable<THook> modifiers,
        Func<THook, Task> action)
        where THook : class
    {
        var modifierSet = new HashSet<THook>(modifiers);
        foreach (var iterateHookListener in cs.IterateHookListeners().OfType<THook>())
        {
            if (!modifierSet.Contains(iterateHookListener)) continue;
            await action(iterateHookListener);
            if (iterateHookListener is AbstractModel model)
                model.InvokeExecutionFinished();
        }
    }


    /// <summary>
    ///     Passes a mutable value through all hook listeners of type <typeparamref name="THook" />,
    ///     tracking which listeners actually modified it via a boolean return value.
    /// </summary>
    public static TValue ModifyMutable<THook, TValue>(
        ICombatState combatState,
        TValue value,
        Func<THook, TValue, bool> amountModifier,
        out IEnumerable<THook> modifiers)
        where THook : class
    {
        var list = combatState.IterateHookListeners().OfType<THook>().Where(model => amountModifier.Invoke(model, value)).ToList();
        modifiers = list;
        return value;
    }
    
    public static Task AfterCustomDraw(ICombatState cs, PlayerChoiceContext ctx, Player player, PileType pile,
        CardPileAddResult result)
    {
        return Dispatch<IAfterCustomDraw>(cs, ctx, m => m.AfterCustomDraw(player, pile, result));
    }
    
    public static Task AfterSoulburnDetonate(ICombatState cs, PlayerChoiceContext ctx, Creature creature)
    {
        return Dispatch<IAfterSoulburnDetonate>(cs, ctx, m => m.AfterSoulburnDetonate(ctx, creature));
    }

    public static Task<bool> ShouldSoulburnDetonateTargetAll(ICombatState cs, PlayerChoiceContext ctx, Creature owner)
    {
        return Task.FromResult(
            Any<IShouldSoulburnDetonateTargetAll>(cs, m => m.ShouldSoulburnDetonateTargetAll(ctx, owner)));
    }
}