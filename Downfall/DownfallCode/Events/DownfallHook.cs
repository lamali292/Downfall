using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
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
    
    public static Task AfterCustomDraw(ICombatState cs, PlayerChoiceContext ctx, Player player, PileType pile,
        CardPileAddResult result)
    {
        return Dispatch<IAfterCustomDraw>(cs, ctx, m => m.AfterCustomDraw(player, pile, result));
    }
}