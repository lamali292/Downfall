using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Downfall.DownfallCode.Events;

/// <summary>
///     Provides utility methods for dispatching and aggregating combat hook events
///     across all <see cref="ICombatState" /> hook listeners.
///     <para />
///     Hook interfaces should be implemented on <see cref="AbstractModel" /> subclasses
///     to be picked up by the listeners.
/// </summary>
public static class MyHookUtils
{
    public enum HookScope
    {
        /// Run-level: hooks on things that live across the whole run, plus combat models when
        /// in combat.
        Run,
 
        /// Combat-level, the normal choice. Runs combat hooks, but skips everything once combat
        /// has started ending (someone won/lost) so hooks don't fire on an already-finished
        /// combat. Combat setup is exempt. Hooks still fire while combat is starting.
        Combat,
 
        /// Combat-level, but without the "is combat ending?" check. hooks fire even while combat
        /// is ending. Important for on kill/death/combat-end logic that needs to run during that window.
        CombatRaw,


    }

    private static IEnumerable<AbstractModel> ResolveListeners(
        HookScope scope,
        ICombatState? combatState = null,
        IRunState? runState = null)
    {
        return scope switch
        {
            HookScope.Run => runState == null ? [] : runState.IterateHookListeners(combatState),
            HookScope.Combat => combatState == null ? [] : Hook.IterateCombatHookListeners(combatState),
            HookScope.CombatRaw => combatState == null ? [] : combatState.IterateHookListeners(),
            _ => throw new ArgumentOutOfRangeException(nameof(scope))
        };
    }



    /// <summary>
    ///     Dispatches an action to all hook listeners of type <typeparamref name="THook" />.
    ///     No-op when <paramref name="combatState" /> is <see langword="null" /> (outside combat).
    /// </summary>
    /// <typeparam name="THook">The hook interface to filter listeners by.</typeparam>
    /// <param name="combatState">The current combat state to iterate listeners from.</param>
    /// <param name="action">The async action to invoke on each matching listener.</param>
    /// <param name="scope">Which listener population to iterate. Pass <see cref="HookScope.Combat" /> for the pre-scope behavior.</param>
    /// <param name="runState">Required only for <see cref="HookScope.Run" />; ignored otherwise.</param>
    public static async Task Dispatch<THook>(ICombatState? combatState, Func<THook, Task> action,
        HookScope scope, IRunState? runState = null)
        where THook : class
    {
        foreach (var model in ResolveListeners(scope, combatState, runState).OfType<THook>())
            await action(model);
    }



    /// <summary>
    ///     Dispatches an action to all combat hook listeners of type <typeparamref name="THook" />,
    ///     pushing and popping each listener onto the provided <see cref="PlayerChoiceContext" />.
    ///     Silently skips listeners that are not <see cref="AbstractModel" /> instances.
    ///     <para>
    ///         No-op when <paramref name="combatState" /> is <see langword="null" />. Unlike
    ///         <see cref="DispatchWithContext{THook}(Player,Func{THook,PlayerChoiceContext,Task},HookScope,IRunState)" />, does not raise
    ///         <see cref="AbstractModel.InvokeExecutionFinished" /> — callers or listeners own that.
    ///     </para>
    /// </summary>
    /// <typeparam name="THook">The hook interface to filter listeners by.</typeparam>
    /// <param name="combatState">The current combat state to iterate listeners from.</param>
    /// <param name="ctx">The player choice context to push/pop each model onto.</param>
    /// <param name="action">The async action to invoke on each matching listener.</param>
    /// <param name="scope">Which listener population to iterate. Pass <see cref="HookScope.Combat" /> for the pre-scope behavior.</param>
    /// <param name="runState">Required only for <see cref="HookScope.Run" />; ignored otherwise.</param>
    public static async Task Dispatch<THook>(ICombatState? combatState, PlayerChoiceContext ctx,
        Func<THook, Task> action, HookScope scope, IRunState? runState = null)
        where THook : class
    {
        foreach (var model in ResolveListeners(scope, combatState, runState).OfType<THook>())
        {
            if (model is not AbstractModel abstractModel) continue;
            ctx.PushModel(abstractModel);
            await action(model);
            ctx.PopModel(abstractModel);
        }
    }

    /// <summary>
    ///     Dispatches an action to all hook listeners of type <typeparamref name="THook" /> in
    ///     <paramref name="player" />'s combat state, creating a
    ///     <see cref="HookPlayerChoiceContext" /> for each listener and awaiting its completion
    ///     or pause. Silently skips listeners that are not <see cref="AbstractModel" /> instances.
    ///     <para>
    ///         No-op when the player has no combat state (outside combat) — this holds for every
    ///         <paramref name="scope" />, including <see cref="HookScope.Run" />, because each
    ///         context is built from the current combat state. Each context is attributed to
    ///         <paramref name="player" />'s <c>NetId</c> — the acting player, not necessarily the
    ///         local client. <see cref="AbstractModel.InvokeExecutionFinished" /> is raised for
    ///         each listener after its action completes; listeners should not raise it themselves.
    ///     </para>
    /// </summary>
    /// <typeparam name="THook">The hook interface to filter listeners by.</typeparam>
    /// <param name="player">
    ///     The player whose combat state supplies the listeners and whose identity the created
    ///     contexts run under.
    /// </param>
    /// <param name="action">
    ///     The async action to invoke on each matching listener, receiving that listener's
    ///     <see cref="PlayerChoiceContext" />.
    /// </param>
    /// <param name="scope">Which listener population to iterate. Pass <see cref="HookScope.Combat" /> for the pre-scope behavior.</param>
    /// <param name="runState">Required only for <see cref="HookScope.Run" />; ignored otherwise.</param>
    public static async Task DispatchWithContext<THook>(Player player,
        Func<THook, PlayerChoiceContext, Task> action,
        HookScope scope, IRunState? runState = null)
        where THook : class
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null) return;
        var netId = player.NetId;
        foreach (var model in ResolveListeners(scope, combatState, runState).OfType<THook>())
        {
            if (model is not AbstractModel abstractModel) continue;
            var hookCtx = new HookPlayerChoiceContext(abstractModel, netId, combatState, GameActionType.Combat);
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
    /// <param name="scope">Which listener population to iterate. Pass <see cref="HookScope.Combat" /> for the pre-scope behavior.</param>
    /// <param name="runState">Required only for <see cref="HookScope.Run" />; ignored otherwise.</param>
    /// <returns>The final aggregated value after all listeners have been processed.</returns>
    public static TResult Aggregate<THook, TResult>(ICombatState? combatState, TResult initial,
        Func<THook, TResult, TResult> action, HookScope scope, IRunState? runState = null)
        where THook : class
        => ResolveListeners(scope, combatState, runState).OfType<THook>()
            .Aggregate(initial, (current, model) => action(model, current));

    /// <summary>
    ///     Returns <see langword="true" /> if all hook listeners of type <typeparamref name="THook" />
    ///     satisfy the given predicate.
    /// </summary>
    /// <typeparam name="THook">The hook interface to filter listeners by.</typeparam>
    /// <param name="combatState">The current combat state to iterate listeners from.</param>
    /// <param name="predicate">The condition to test each listener against.</param>
    /// <param name="scope">Which listener population to iterate. Pass <see cref="HookScope.Combat" /> for the pre-scope behavior.</param>
    /// <param name="runState">Required only for <see cref="HookScope.Run" />; ignored otherwise.</param>
    public static bool All<THook>(ICombatState? combatState, Func<THook, bool> predicate,
        HookScope scope, IRunState? runState = null)
        where THook : class
        => ResolveListeners(scope, combatState, runState).OfType<THook>().All(predicate);



    /// <summary>
    ///     Returns <see langword="true" /> if all hook listeners of type <typeparamref name="THook" />
    ///     satisfy the given predicate, additionally collecting the listeners that failed it.
    ///     Vacuously <see langword="true" /> when no listeners of the type exist.
    /// </summary>
    /// <typeparam name="THook">The hook interface to filter listeners by.</typeparam>
    /// <param name="combatState">The current combat state to iterate listeners from.</param>
    /// <param name="predicate">The condition to test each listener against.</param>
    /// <param name="nonMatches">
    ///     The listeners that did <b>not</b> satisfy the predicate; empty when the result is
    ///     <see langword="true" />.
    /// </param>
    /// <param name="scope">Which listener population to iterate. Pass <see cref="HookScope.Combat" /> for the pre-scope behavior.</param>
    /// <param name="runState">Required only for <see cref="HookScope.Run" />; ignored otherwise.</param>
    public static bool All<THook>(ICombatState? combatState, Func<THook, bool> predicate,
        out IEnumerable<THook> nonMatches, HookScope scope, IRunState? runState = null)
        where THook : class
    {
        var list = ResolveListeners(scope, combatState, runState).OfType<THook>()
            .Where(m => !predicate(m)).ToList();
        nonMatches = list;
        return list.Count == 0;
    }

    /// <summary>
    ///     Returns <see langword="true" /> if any hook listener of type <typeparamref name="THook" />
    ///     satisfies the given predicate.
    /// </summary>
    /// <typeparam name="THook">The hook interface to filter listeners by.</typeparam>
    /// <param name="combatState">The current combat state to iterate listeners from.</param>
    /// <param name="predicate">The condition to test each listener against.</param>
    /// <param name="scope">Which listener population to iterate. Pass <see cref="HookScope.Combat" /> for the pre-scope behavior.</param>
    /// <param name="runState">Required only for <see cref="HookScope.Run" />; ignored otherwise.</param>
    public static bool Any<THook>(ICombatState? combatState, Func<THook, bool> predicate,
        HookScope scope, IRunState? runState = null)
        where THook : class
        => ResolveListeners(scope, combatState, runState).OfType<THook>().Any(predicate);

    /// <summary>
    ///     Returns <see langword="true" /> if any hook listener of type <typeparamref name="THook" />
    ///     satisfies the given predicate, additionally collecting all listeners that matched.
    ///     Unlike LINQ <c>Any</c>, this does not short-circuit — the predicate runs for every listener.
    /// </summary>
    /// <typeparam name="THook">The hook interface to filter listeners by.</typeparam>
    /// <param name="combatState">The current combat state to iterate listeners from.</param>
    /// <param name="predicate">The condition to test each listener against.</param>
    /// <param name="matches">
    ///     All listeners that satisfied the predicate; empty when the result is
    ///     <see langword="false" />.
    /// </param>
    /// <param name="scope">Which listener population to iterate. Pass <see cref="HookScope.Combat" /> for the pre-scope behavior.</param>
    /// <param name="runState">Required only for <see cref="HookScope.Run" />; ignored otherwise.</param>
    public static bool Any<THook>(ICombatState? combatState, Func<THook, bool> predicate,
        out IEnumerable<THook> matches, HookScope scope, IRunState? runState = null)
        where THook : class
    {
        var list = ResolveListeners(scope, combatState, runState).OfType<THook>()
            .Where(predicate).ToList();
        matches = list;
        return list.Count > 0;
    }

    /// <summary>
    ///     Passes a value through all hook listeners of type <typeparamref name="THook" />,
    ///     tracking which listeners changed it.
    /// </summary>
    /// <typeparam name="THook">The hook interface to filter listeners by.</typeparam>
    /// <typeparam name="TValue">The type of the value being modified. Must implement <see cref="IEquatable{T}" />.</typeparam>
    /// <param name="combatState">The current combat state to iterate listeners from.</param>
    /// <param name="originalAmount">The initial value before any modifications.</param>
    /// <param name="amountModifier">A function that takes a listener and the current value and returns the modified value.</param>
    /// <param name="modifiers">
    ///     Outputs the listeners whose call changed the value they received (per-step
    ///     <typeparamref name="TValue" /> equality). Listeners returning their input unchanged are
    ///     excluded; listeners whose changes later cancel out are <b>included</b>, so this set can
    ///     be non-empty even when the returned value equals <paramref name="originalAmount" />.
    /// </param>
    /// <param name="scope">Which listener population to iterate. Pass <see cref="HookScope.Combat" /> for the pre-scope behavior.</param>
    /// <param name="runState">Required only for <see cref="HookScope.Run" />; ignored otherwise.</param>
    /// <returns>
    ///     The final modified value. When <paramref name="combatState" /> is
    ///     <see langword="null" />, returns <paramref name="originalAmount" /> with an empty
    ///     <paramref name="modifiers" /> set.
    /// </returns>
    public static TValue Modify<THook, TValue>(
        ICombatState? combatState,
        TValue originalAmount,
        Func<THook, TValue, TValue> amountModifier,
        out IEnumerable<THook> modifiers,
        HookScope scope,
        IRunState? runState = null)
        where THook : class
        where TValue : IEquatable<TValue>
    {
        var amount = originalAmount;
        var abstractModelList = new List<THook>();
        foreach (var model in ResolveListeners(scope, combatState, runState).OfType<THook>())
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
    ///     Invokes a follow-up action on the listeners that previously modified a value via
    ///     <see cref="Modify{THook,TValue}(ICombatState,TValue,Func{THook,TValue,TValue},out IEnumerable{THook},HookScope,IRunState)" />, iterating in current hook-listener order (not the
    ///     order of <paramref name="modifiers" />). Listeners no longer present in the combat
    ///     state's iteration are silently skipped.
    ///     <see cref="AbstractModel.InvokeExecutionFinished" /> is raised after each action for
    ///     listeners that are <see cref="AbstractModel" /> instances; implementations should not
    ///     raise it themselves.
    /// </summary>
    /// <typeparam name="THook">The hook interface to filter listeners by.</typeparam>
    /// <param name="cs">The current combat state to iterate listeners from.</param>
    /// <param name="modifiers">
    ///     The set of listeners that modified the value, as returned by
    ///     <see cref="Modify{THook,TValue}(ICombatState,TValue,Func{THook,TValue,TValue},out IEnumerable{THook},HookScope,IRunState)" />.
    /// </param>
    /// <param name="action">The async action to invoke on each modifier.</param>
    /// <param name="scope">Which listener population to iterate. Use the same scope that produced <paramref name="modifiers" />.</param>
    /// <param name="runState">Required only for <see cref="HookScope.Run" />; ignored otherwise.</param>
    public static async Task AfterModifying<THook>(ICombatState? cs, IEnumerable<THook> modifiers,
        Func<THook, Task> action, HookScope scope, IRunState? runState = null)
        where THook : class
    {
        var modifierSet = new HashSet<THook>(modifiers);
        foreach (var iterateHookListener in ResolveListeners(scope, cs, runState).OfType<THook>())
        {
            if (!modifierSet.Contains(iterateHookListener)) continue;
            await action(iterateHookListener);
            if (iterateHookListener is AbstractModel model)
                model.InvokeExecutionFinished();
        }
    }

    /// <summary>
    ///     Presents a mutable <paramref name="value" /> to all hook listeners of type
    ///     <typeparamref name="THook" /> for in-place modification. Every listener is invoked
    ///     exactly once (the enumeration is fully materialized); each returns
    ///     <see langword="true" /> to declare that it modified the value.
    ///     <para>
    ///         Unlike <see cref="Modify{THook,TValue}(ICombatState,TValue,Func{THook,TValue,TValue},out IEnumerable{THook},HookScope,IRunState)" />, modification tracking is
    ///         <b>self-reported</b> — nothing verifies the value actually changed, so listeners
    ///         must return honestly or the <see cref="AfterModifying{THook}(ICombatState,IEnumerable{THook},Func{THook,Task},HookScope,IRunState)" /> follow-up will
    ///         target the wrong set. The same <paramref name="value" /> instance is returned;
    ///         it is passed back for call-site fluency, not copied.
    ///     </para>
    /// </summary>
    /// <typeparam name="THook">The hook interface to filter listeners by.</typeparam>
    /// <typeparam name="TValue">The mutable type being modified in place (typically a class or collection).</typeparam>
    /// <param name="combatState">The current combat state to iterate listeners from.</param>
    /// <param name="value">The instance listeners may mutate.</param>
    /// <param name="amountModifier">
    ///     Invoked per listener with the shared instance; returns whether this listener modified it.
    /// </param>
    /// <param name="modifiers">The listeners that reported modifying the value.</param>
    /// <param name="scope">Which listener population to iterate. Pass <see cref="HookScope.Combat" /> for the pre-scope behavior.</param>
    /// <param name="runState">Required only for <see cref="HookScope.Run" />; ignored otherwise.</param>
    /// <returns>The same <paramref name="value" /> instance, after all listeners ran.</returns>
    public static TValue ModifyMutable<THook, TValue>(
        ICombatState? combatState,
        TValue value,
        Func<THook, TValue, bool> amountModifier,
        out IEnumerable<THook> modifiers,
        HookScope scope,
        IRunState? runState = null)
        where THook : class
    {
        var list = ResolveListeners(scope, combatState, runState).OfType<THook>()
            .Where(model => amountModifier.Invoke(model, value)).ToList();
        modifiers = list;
        return value;
    }

    
    
    // Combat-scoped shorthands.
    // because of backwards compatibility, we can't use `HookScope scope = HookScope.Combat` as the default value above
    // as this would change signature.

    /// <summary>
    ///     Combat-scoped shorthand for
    ///     <see cref="Dispatch{THook}(ICombatState,Func{THook,Task},HookScope,IRunState)" />
    ///     with <see cref="HookScope.Combat" />.
    /// </summary>
    public static Task Dispatch<THook>(ICombatState? combatState, Func<THook, Task> action)
        where THook : class
        => Dispatch(combatState, action, HookScope.Combat);

    /// <summary>
    ///     Combat-scoped shorthand for
    ///     <see cref="Dispatch{THook}(ICombatState,PlayerChoiceContext,Func{THook,Task},HookScope,IRunState)" />
    ///     with <see cref="HookScope.Combat" />.
    /// </summary>
    public static Task Dispatch<THook>(ICombatState? combatState, PlayerChoiceContext ctx,
        Func<THook, Task> action)
        where THook : class
        => Dispatch(combatState, ctx, action, HookScope.Combat);

    /// <summary>
    ///     Combat-scoped shorthand for
    ///     <see cref="DispatchWithContext{THook}(Player,Func{THook,PlayerChoiceContext,Task},HookScope,IRunState)" />
    ///     with <see cref="HookScope.Combat" />.
    /// </summary>
    public static Task DispatchWithContext<THook>(Player player,
        Func<THook, PlayerChoiceContext, Task> action)
        where THook : class
        => DispatchWithContext(player, action, HookScope.Combat);

    /// <summary>
    ///     Combat-scoped shorthand for
    ///     <see cref="Aggregate{THook,TResult}(ICombatState,TResult,Func{THook,TResult,TResult},HookScope,IRunState)" />
    ///     with <see cref="HookScope.Combat" />.
    /// </summary>
    public static TResult Aggregate<THook, TResult>(ICombatState combatState, TResult initial,
        Func<THook, TResult, TResult> action)
        where THook : class
        => Aggregate(combatState, initial, action, HookScope.Combat);

    /// <summary>
    ///     Combat-scoped shorthand for
    ///     <see cref="All{THook}(ICombatState,Func{THook,bool},HookScope,IRunState)" />
    ///     with <see cref="HookScope.Combat" />.
    /// </summary>
    public static bool All<THook>(ICombatState combatState, Func<THook, bool> predicate)
        where THook : class
        => All(combatState, predicate, HookScope.Combat);

    /// <summary>
    ///     Combat-scoped shorthand for
    ///     <see cref="All{THook}(ICombatState,Func{THook,bool},out IEnumerable{THook},HookScope,IRunState)" />
    ///     with <see cref="HookScope.Combat" />.
    /// </summary>
    public static bool All<THook>(ICombatState combatState, Func<THook, bool> predicate,
        out IEnumerable<THook> nonMatches)
        where THook : class
        => All(combatState, predicate, out nonMatches, HookScope.Combat);

    /// <summary>
    ///     Combat-scoped shorthand for
    ///     <see cref="Any{THook}(ICombatState,Func{THook,bool},HookScope,IRunState)" />
    ///     with <see cref="HookScope.Combat" />.
    /// </summary>
    public static bool Any<THook>(ICombatState combatState, Func<THook, bool> predicate)
        where THook : class
        => Any(combatState, predicate, HookScope.Combat);

    /// <summary>
    ///     Combat-scoped shorthand for
    ///     <see cref="Any{THook}(ICombatState,Func{THook,bool},out IEnumerable{THook},HookScope,IRunState)" />
    ///     with <see cref="HookScope.Combat" />.
    /// </summary>
    public static bool Any<THook>(ICombatState combatState, Func<THook, bool> predicate,
        out IEnumerable<THook> matches)
        where THook : class
        => Any(combatState, predicate, out matches, HookScope.Combat);

    /// <summary>
    ///     Combat-scoped shorthand for
    ///     <see cref="Modify{THook,TValue}(ICombatState,TValue,Func{THook,TValue,TValue},out IEnumerable{THook},HookScope,IRunState)" />
    ///     with <see cref="HookScope.Combat" />.
    /// </summary>
    public static TValue Modify<THook, TValue>(
        ICombatState? combatState,
        TValue originalAmount,
        Func<THook, TValue, TValue> amountModifier,
        out IEnumerable<THook> modifiers)
        where THook : class
        where TValue : IEquatable<TValue>
        => Modify(combatState, originalAmount, amountModifier, out modifiers, HookScope.Combat);

    /// <summary>
    ///     Combat-scoped shorthand for
    ///     <see cref="AfterModifying{THook}(ICombatState,IEnumerable{THook},Func{THook,Task},HookScope,IRunState)" />
    ///     with <see cref="HookScope.Combat" />.
    /// </summary>
    public static Task AfterModifying<THook>(ICombatState cs, IEnumerable<THook> modifiers,
        Func<THook, Task> action)
        where THook : class
        => AfterModifying(cs, modifiers, action, HookScope.Combat);

    /// <summary>
    ///     Combat-scoped shorthand for
    ///     <see cref="ModifyMutable{THook,TValue}(ICombatState,TValue,Func{THook,TValue,bool},out IEnumerable{THook},HookScope,IRunState)" />
    ///     with <see cref="HookScope.Combat" />.
    /// </summary>
    public static TValue ModifyMutable<THook, TValue>(
        ICombatState combatState,
        TValue value,
        Func<THook, TValue, bool> amountModifier,
        out IEnumerable<THook> modifiers)
        where THook : class
        => ModifyMutable(combatState, value, amountModifier, out modifiers, HookScope.Combat);
}