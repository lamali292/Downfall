using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.Metrics;
using MegaCrit.Sts2.Core.Saves;

namespace Downfall.DownfallCode.Compatibility;

/// <summary>
///     Run-metrics API drift: newer builds add <c>RunMetrics.GameMode</c> as a <c>required</c> member and give
///     <c>MetricUtilities.ShouldUploadMetrics</c> an <c>isRunMetrics</c> parameter.
/// </summary>
public static class RunMetricsCompat
{
    private static readonly PropertyInfo? GameModeProp = AccessTools.Property(typeof(RunMetrics), "GameMode");

    private static readonly PropertyInfo? RunGameModeProp = AccessTools.Property(typeof(SerializableRun), "GameMode");

    private static readonly MethodInfo? ShouldUploadMethod =
        AccessTools.Method(typeof(MetricUtilities), "ShouldUploadMetrics");

    /// <summary>
    ///     Fills <c>RunMetrics.GameMode</c> (string like "STANDARD") when this game version has it; no-op otherwise.
    /// </summary>
    public static void ApplyGameMode(RunMetrics metrics, SerializableRun run)
    {
        if (GameModeProp == null) return;
        var mode = RunGameModeProp?.GetValue(run)?.ToString()?.ToUpperInvariant() ?? "STANDARD";
        GameModeProp.SetValue(metrics, mode);
    }

    /// <summary>Calls <c>MetricUtilities.ShouldUploadMetrics</c> with whichever signature this version has.</summary>
    public static bool ShouldUploadRunMetrics()
    {
        if (ShouldUploadMethod == null) return false;
        object?[] args = ShouldUploadMethod.GetParameters().Length == 0 ? [] : [true];
        return ShouldUploadMethod.Invoke(null, args) as bool? ?? false;
    }
}

/// <summary>
///     <see cref="RunMetrics"/> whose constructor claims to set all required members, so it can be built with an
///     object initializer on game versions that add new <c>required</c> properties (e.g. <c>GameMode</c>).
///     Newly added members are filled afterwards via <see cref="RunMetricsCompat"/>.
/// </summary>
public sealed class CompatRunMetrics : RunMetrics
{
#pragma warning disable CS8618 // the call site's object initializer fills the base type's required members
    [SetsRequiredMembers]
    public CompatRunMetrics()
    {
    }
#pragma warning restore CS8618
}
