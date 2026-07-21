using System.Reflection;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.DownfallCode.Utils.UI;

public interface ITopBarElementDescriptor
{
    string ScenePath { get; }
    float Width { get; }
    bool CanUse(Player player);
}

public interface ITopBarElement
{
    void Initialize(Player player);
}

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