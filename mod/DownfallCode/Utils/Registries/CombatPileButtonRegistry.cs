using System.Reflection;
using System.Runtime.CompilerServices;
using Downfall.DownfallCode.Utils.UI;

namespace Downfall.DownfallCode.Utils;

internal static class CombatPileButtonRegistry
{
    private static List<Type>? _types;

    internal static IReadOnlyList<Type> Types => _types ??= Discover();

    private static List<Type> Discover()
    {
        var results = new List<Type>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            IEnumerable<Type> types;
            try { types = assembly.GetTypes(); }
            catch (ReflectionTypeLoadException ex) { types = ex.Types.Where(t => t != null)!; }

            results.AddRange(types.Where(t =>
                t is { IsClass: true, IsAbstract: false } &&
                t.IsSubclassOf(typeof(NCustomCombatCardPile))));
        }
        return results;
    }

    internal static string ReadMetadata(Type type)
    {
        var probe = (NCustomCombatCardPile)RuntimeHelpers.GetUninitializedObject(type);
        return probe.ScenePath;
    }
}