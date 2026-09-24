using System.Reflection;

namespace Downfall.DownfallCode.Utils.UI;

internal static class TopBarElementRegistry
{
    private static List<Type>? _types;
    internal static IReadOnlyList<Type> Types => _types ??= Discover();

    private static List<Type> Discover()
    {
        var results = new List<Type>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            IEnumerable<Type> types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null)!;
            }

            results.AddRange(types.Where(t =>
                t is { IsClass: true, IsAbstract: false } &&
                t.IsAssignableTo(typeof(ITopBarElementDescriptor))));
        }

        return results;
    }
}
