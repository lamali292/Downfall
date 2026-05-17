using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Events;

public static class DownfallHook
{
    public static async Task Dispatch<T>(ICombatState combatState, Func<T, Task> action)
        where T : class
    {
        foreach (var model in combatState.IterateHookListeners().OfType<T>())
            await action(model);
    }

    public static async Task Dispatch<T>(ICombatState combatState, PlayerChoiceContext ctx, Func<T, Task> action)
        where T : class
    {
        foreach (var model in combatState.IterateHookListeners().OfType<T>())
        {
            var abstractModel = (AbstractModel)(object)model;
            ctx.PushModel(abstractModel);
            await action(model);
            ctx.PopModel(abstractModel);
        }
    }

    public static async Task DispatchHookCtx<T>(ICombatState combatState, Func<T, PlayerChoiceContext, Task> action)
        where T : class
    {
        var netId = LocalContext.NetId;
        if (!netId.HasValue) return;

        foreach (var model in combatState.IterateHookListeners().OfType<T>())
        {
            var abstractModel = (AbstractModel)(object)model;
            var hookCtx = new HookPlayerChoiceContext(abstractModel, netId.Value, combatState, GameActionType.Combat);
            var task = action(model, hookCtx);
            await hookCtx.AssignTaskAndWaitForPauseOrCompletion(task);
            abstractModel.InvokeExecutionFinished();
        }
    }

    public static TResult Aggregate<T, TResult>(ICombatState combatState, TResult seed,
        Func<T, TResult, TResult> action)
        where T : class
    {
        return combatState.IterateHookListeners().OfType<T>()
            .Aggregate(seed, (current, model) => action(model, current));
    }


    public static bool Any<T>(ICombatState combatState, Func<T, bool> predicate)
        where T : class
    {
        return combatState.IterateHookListeners().OfType<T>().Any(predicate);
    }

    public static bool All<T>(ICombatState combatState, Func<T, bool> predicate)
        where T : class
    {
        return combatState.IterateHookListeners().OfType<T>().All(predicate);
    }

    public static TW Modify<T, TW>(
        ICombatState combatState,
        TW originalAmount,
        Func<T, TW, TW> amountModifier,
        out IEnumerable<T> modifiers)
        where T : class
        where TW : IEquatable<TW>
    {
        var amount = originalAmount;
        var abstractModelList = new List<T>();
        foreach (var model in combatState.IterateHookListeners().OfType<T>())
        {
            var previous = amount;
            amount = amountModifier.Invoke(model, amount);
            if (!previous.Equals(amount))
                abstractModelList.Add(model);
        }

        modifiers = abstractModelList;
        return amount;
    }

    public static async Task AfterModifying<T>(ICombatState cs, IEnumerable<T> modifiers, Func<T, Task> action)
        where T : class
    {
        var modifierSet = new HashSet<T>(modifiers);
        foreach (var iterateHookListener in cs.IterateHookListeners().OfType<T>())
        {
            if (!modifierSet.Contains(iterateHookListener)) continue;
            await action(iterateHookListener);
            if (iterateHookListener is AbstractModel model)
                model.InvokeExecutionFinished();
        }
    }

    public static Task AfterCustomDraw(ICombatState cs, PlayerChoiceContext ctx, Player player, PileType pile,
        CardPileAddResult result)
    {
        return Dispatch<IAfterCustomDraw>(cs, ctx, m => m.AfterCustomDraw(player, pile, result));
    }
}