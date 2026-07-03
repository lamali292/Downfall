using System.Reflection;
using MegaCrit.Sts2.Core.Modding;

namespace Downfall.DownfallCode.Compatibility;

public static class CompatibilityMod
{
    public static List<Assembly> GetAssemblies(this Mod mod)
    {
#if V107
         return mod.assembly == null ? [] : [mod.assembly];
#else
        return mod.assemblies;
#endif

    }
}