using SmartFormat;
using SmartFormat.Core.Extensions;

namespace Downfall.DownfallCode.Utils;

public static class LocFormatterRegistry
{
    private static readonly List<IFormatter> formatters = new();
    private static bool loaded;

    public static void Register(params IFormatter[] items)
    {
        formatters.AddRange(items);
        if (loaded) Smart.Default.AddExtensions(items);
    }

    internal static void ApplyAll()
    {
        loaded = true;
        Smart.Default.AddExtensions(formatters.ToArray());
    }
}