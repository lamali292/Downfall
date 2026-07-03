using System.Reflection;
using MegaCrit.Sts2.Core.Modding;

namespace Downfall.DownfallCode.Compatibility;

public static class CompatibilityMod
{
    private static readonly Func<Mod, List<Assembly>> _getAssemblies = BuildAccessor();

    public static List<Assembly> GetAssemblies(this Mod mod) => _getAssemblies(mod);

    private static Func<Mod, List<Assembly>> BuildAccessor()
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        var type = typeof(Mod);

        // New API: "assemblies" (List<Assembly> or IEnumerable<Assembly>)
        var multi = (MemberInfo?)type.GetProperty("assemblies", flags)
                    ?? type.GetField("assemblies", flags);
        if (multi != null)
        {
            return mod => GetValue(multi, mod) switch
            {
                List<Assembly> list => list,
                IEnumerable<Assembly> seq => seq.ToList(),
                _ => []
            };
        }

        // Old API (V107): "assembly" (single Assembly, possibly null)
        var single = (MemberInfo?)type.GetProperty("assembly", flags)
                     ?? type.GetField("assembly", flags);
        if (single != null)
        {
            return mod => GetValue(single, mod) is Assembly asm ? [asm] : [];
        }

        throw new MissingMemberException(
            "Mod has neither an 'assemblies' nor an 'assembly' member — unsupported game version.");
    }

    private static object? GetValue(MemberInfo member, Mod mod) => member switch
    {
        PropertyInfo p => p.GetValue(mod),
        FieldInfo f => f.GetValue(mod),
        _ => null
    };
}