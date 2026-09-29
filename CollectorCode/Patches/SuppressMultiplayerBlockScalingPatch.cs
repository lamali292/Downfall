using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Singleton;

namespace Collector.CollectorCode.Patches;

/// <summary>
///     Lets a block gain run through every block hook except <see cref="MultiplayerScalingModel" />.
///     Wrap the gain in <see cref="MultiplayerBlockScaling.Suppress" />; the scope is an <see cref="AsyncLocal{T}" />,
///     so it only affects the awaited gain and not unrelated block gains running elsewhere.
/// </summary>
[HarmonyPatch(typeof(MultiplayerScalingModel), nameof(MultiplayerScalingModel.ModifyBlockMultiplicative))]
public static class SuppressMultiplayerBlockScalingPatch
{
    static bool Prefix(ref decimal __result)
    {
        if (!MultiplayerBlockScaling.IsSuppressed) return true;
        __result = 1m;
        return false;
    }
}

public static class MultiplayerBlockScaling
{
    private static readonly AsyncLocal<int> Depth = new();

    public static bool IsSuppressed => Depth.Value > 0;

    public static async Task<T> Suppress<T>(Func<Task<T>> action)
    {
        Depth.Value++;
        try
        {
            return await action();
        }
        finally
        {
            Depth.Value--;
        }
    }
}
