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
///     when AddNextState was originally called.
/// </summary>
public static class AnimStateNextStateCompat
{
    private static readonly ConditionalWeakTable<AnimState, List<(AnimState state, Func<bool> when)>> Branches = new();

    /// <summary>Registers `to` as a next-state candidate for `from`, picked when `when` is true (first match wins).</summary>
    public static void AddNextState(this AnimState from, AnimState to, Func<bool> when)
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
