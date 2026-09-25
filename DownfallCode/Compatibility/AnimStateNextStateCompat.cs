using System.Runtime.CompilerServices;
using Downfall.DownfallCode;
using HarmonyLib;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Compatibility;

/// <summary>
///     AnimState.NextState is a single fixed reference, baked in whenever a non-looping animation's
///     trigger fires — it can't natively depend on runtime state (e.g. which idle variant a cast/attack/
///     hurt anim should return to: idle, idle_awakened, idle_defensive, ...). This adds conditional
///     candidates on top of it. Freshness is handled by <see cref="RefreshConditionalNextStatesPatch"/>,
///     which re-evaluates every registered AnimState's NextState right before any animation trigger call,
///     so it's always current by the time a trigger actually reads it - not just whatever was true when
///     AddConditionalNextState was originally called.
///
///     The refresh patch targets <c>NCreature.SetAnimationTrigger</c>, NOT <c>CreatureAnimator.SetTrigger</c>
///     itself. `SetTrigger` is small enough that the shipped game build inlines its body directly into its
///     only caller (`SetAnimationTrigger`) - Harmony still reports the patch as successfully attached (it
///     patches the method's own compiled body), but since there's no discrete call left at the call site to
///     redirect, the prefix silently never runs. Confirmed by instrumenting both methods: a prefix on
///     `SetAnimationTrigger` fires for every trigger every time, while one on `SetTrigger` never fires at
///     all despite `Harmony.GetPatchInfo` showing it attached. This is the same class of bug documented in
///     DownfallCode/CLAUDE.md for patching async kickoff stubs, just triggered by ordinary AOT/JIT inlining
///     of a tiny method instead. Don't move this back to `CreatureAnimator.SetTrigger`.
///
///     One known gap: `NCreature.ImmediatelySetIdle` (revive only) calls `_spineAnimator.SetTrigger("Idle")`
///     directly, bypassing `SetAnimationTrigger` - stance/mode NextStates won't get refreshed by that path,
///     but it only matters for the moment a creature revives, and is fine to leave in setup order.
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

    private static void Refresh(AnimState from, List<(AnimState state, Func<bool> when)> candidates)
    {
        foreach (var (state, when) in candidates)
        {
            bool matches;
            try
            {
                matches = when();
            }
            catch (Exception ex)
            {
                // A single stale/dead creature reference in one candidate's condition must not
                // stop RefreshAll from updating every other registered AnimState (see RefreshAll).
                DownfallMainFile.Logger.Warn(
                    $"AnimStateNextStateCompat: condition for '{from.Id}' -> '{state.Id}' threw, skipping candidate. {ex.Message}");
                continue;
            }

            if (!matches) continue;
            from.NextState = state;
            return;
        }
    }

    internal static void RefreshAll()
    {
        // Isolated per-entry: one AnimState's condition throwing (e.g. a creature reference that's
        // gone stale) must not prevent every other registered AnimState from being refreshed too -
        // otherwise an unrelated character's animator can end up reading a stale NextState right
        // when a SetTrigger call actually needs it.
        foreach (var (state, candidates) in Branches)
        {
            try
            {
                Refresh(state, candidates);
            }
            catch (Exception ex)
            {
                DownfallMainFile.Logger.Warn($"AnimStateNextStateCompat: failed to refresh '{state.Id}'. {ex.Message}");
            }
        }
    }
}

/// <summary>
///     Keeps every AddNextState-registered AnimState's NextState current before it's read. Targets
///     `NCreature.SetAnimationTrigger` rather than `CreatureAnimator.SetTrigger` - see the class doc above
///     for why patching `SetTrigger` directly silently doesn't work.
/// </summary>
[HarmonyPatch(typeof(NCreature), nameof(NCreature.SetAnimationTrigger))]
internal static class RefreshConditionalNextStatesPatch
{
    private static void Prefix() => AnimStateNextStateCompat.RefreshAll();
}
