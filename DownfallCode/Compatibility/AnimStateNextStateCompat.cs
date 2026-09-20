using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Animation;

namespace Downfall.DownfallCode.Compatibility;

/// <summary>
///     AnimState.NextState is a single fixed reference, baked in whenever a non-looping animation's
///     trigger fires — it can't natively depend on runtime state (e.g. which idle variant a cast/attack/
///     hurt anim should return to: idle, idle_awakened, idle_defensive, ...). This adds conditional
///     candidates on top of it. Freshness is handled by <see cref="RefreshConditionalNextStatesPatch"/>,
///     which re-evaluates every registered AnimState's NextState right before any CreatureAnimator.SetTrigger
///     call, so it's always current by the time a trigger actually reads it - not just whatever was true
///     when AddConditionalNextState was originally called.
///
///     Deliberately NOT named "AddNextState": a same-named, same-signature extension is silently shadowed
///     (no compile error) if the game itself ever adds a native instance method with that name - which is
///     exactly what happened on the beta branch, causing DLLs built against beta's sts2.dll to bake in a
///     direct call to the native method and MissingMethodException when run against main, which had no such
///     method. Keep this name distinct from anything the game is likely to add.
/// </summary>
public static class AnimStateNextStateCompat
{
    private static readonly ConditionalWeakTable<AnimState, List<(AnimState state, Func<bool> when)>> Branches = new();

    /// <summary>Registers `to` as a next-state candidate for `from`, picked when `when` is true (first match wins).</summary>
    public static void AddConditionalNextState(this AnimState from, AnimState to, Func<bool> when)
    {
        var candidates = Branches.GetOrCreateValue(from);
        candidates.Add((to, when));
        Refresh(from, candidates);
    }

    private static void Refresh(AnimState from, List<(AnimState state, Func<bool> when)> candidates) =>
        from.NextState = candidates.FirstOrDefault(c => c.when()).state ?? from.NextState;

    internal static void RefreshAll()
    {
        foreach (var (state, candidates) in Branches)
            Refresh(state, candidates);
    }
}

/// <summary>Keeps every AddNextState-registered AnimState's NextState current before it's read.</summary>
[HarmonyPatch(typeof(CreatureAnimator), nameof(CreatureAnimator.SetTrigger))]
internal static class RefreshConditionalNextStatesPatch
{
    private static void Prefix() => AnimStateNextStateCompat.RefreshAll();
}
