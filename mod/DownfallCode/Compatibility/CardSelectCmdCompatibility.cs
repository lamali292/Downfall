using System.Linq.Expressions;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;

namespace Downfall.DownfallCode.Compatibility;

/// <summary>
///     Version-safe access to the automated (test) card selector. Older builds keep it in static
///     <c>CardSelectCmd.Selector/UseSelector</c>; newer ones moved it into <c>CardSelectorStack</c>
///     (<c>RunManager.CardSelectorStack</c> per player, plus the static <c>SetTestSelector</c> fallback).
/// </summary>
public static class CardSelectCmdCompatibility
{
    private static readonly PropertyInfo? SelectorProp = AccessTools.Property(typeof(CardSelectCmd), "Selector");

    private static readonly Type? StackType =
        AccessTools.TypeByName("MegaCrit.Sts2.Core.CardSelection.CardSelectorStack");

    private static readonly MethodInfo? GetSelectorMethod = StackType == null
        ? null
        : AccessTools.Method(StackType, "GetSelector", [typeof(Player)]);

    private static readonly PropertyInfo? StackProp = AccessTools.Property(typeof(RunManager), "CardSelectorStack");

    private static readonly UseSelectorDel UseSelectorImpl = BuildUseSelector();

    /// <summary>The automated selector currently active for <paramref name="player"/>, or null if none.</summary>
    public static ICardSelector? GetCurrentSelector(Player player)
    {
        if (SelectorProp != null)
            return SelectorProp.GetValue(null) as ICardSelector;

        var stack = StackProp?.GetValue(RunManager.Instance);
        return stack == null ? null : GetSelectorMethod?.Invoke(stack, [player]) as ICardSelector;
    }

    /// <summary>
    ///     Installs a test-only ICardSelector. Newer game versions dropped the
    ///     `localOnly` parameter; on those, it's simply ignored.
    /// </summary>
    public static IDisposable UseSelector(ICardSelector selector, bool localOnly = false) =>
        UseSelectorImpl(selector, localOnly);

    private static UseSelectorDel BuildUseSelector()
    {
        var selector = Expression.Parameter(typeof(ICardSelector), "selector");
        var localOnly = Expression.Parameter(typeof(bool), "localOnly");

        var twoArgMethod = typeof(CardSelectCmd).GetMethod("UseSelector",
            BindingFlags.Public | BindingFlags.Static, null, [typeof(ICardSelector), typeof(bool)], null);
        if (twoArgMethod != null)
        {
            var call = Expression.Call(twoArgMethod, selector, localOnly);
            return Expression.Lambda<UseSelectorDel>(call, selector, localOnly).Compile();
        }

        var oneArgMethod = typeof(CardSelectCmd).GetMethod("UseSelector",
            BindingFlags.Public | BindingFlags.Static, null, [typeof(ICardSelector)], null);
        // Newest builds: static CardSelectorStack.SetTestSelector(selector) has the same one-arg shape.
        oneArgMethod ??= StackType?.GetMethod("SetTestSelector",
            BindingFlags.Public | BindingFlags.Static, null, [typeof(ICardSelector)], null);
        if (oneArgMethod == null)
            throw new MissingMethodException("No test card selector installer found (CardSelectCmd.UseSelector / CardSelectorStack.SetTestSelector)");

        var oneArgCall = Expression.Call(oneArgMethod, selector);
        var oneArgDel = Expression.Lambda<OneArgUseSelectorDel>(oneArgCall, selector).Compile();
        return (s, _) => oneArgDel(s);
    }

    private delegate IDisposable UseSelectorDel(ICardSelector selector, bool localOnly);

    private delegate IDisposable OneArgUseSelectorDel(ICardSelector selector);
}
