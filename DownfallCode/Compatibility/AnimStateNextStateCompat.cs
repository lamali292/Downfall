using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Compatibility;

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
