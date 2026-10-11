using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Modding;

namespace Downfall.DownfallCode.Compatibility;

public static class ModManagerCompat
{
    private static readonly MethodInfo? AssociateAssemblyWithModMethod = typeof(ModManager).GetMethod(
        "AssociateAssemblyWithMod",
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
        [typeof(string), typeof(Assembly)]);

    // Older builds (V107) only know one assembly per mod: ReflectionHelper.ModTypes (ModelDb's content discovery)
    // and ModelIdSerializationCache.Init (net IDs, via ReflectionHelper.GetSubtypesFromAssembly(mod.assembly))
    // both read exactly that one. Assemblies we load by hand for our internal submods are therefore invisible
    // unless we append them to both ourselves, as part of Downfall's own assembly.
    private static readonly List<Assembly> ExtraAssemblies = [];
    private static Type[]? _lastModTypes;
    private static Type[] _mergedModTypes = [];
    private static bool _patched;

    /// <summary>
    /// Version-safe ModManager.AssociateAssemblyWithMod. Where the game has it, it is simply called.
    /// Where it doesn't (V107), the assembly's types are merged into ReflectionHelper.ModTypes instead.
    /// </summary>
    public static void AssociateAssemblyWithMod(string modId, Assembly assembly)
    {
        if (AssociateAssemblyWithModMethod != null)
        {
            AssociateAssemblyWithModMethod.Invoke(null, [modId, assembly]);
            return;
        }

        ExtraAssemblies.Add(assembly);
        _lastModTypes = null; // force the merged array to be rebuilt on the next ModTypes read
        if (_patched) return;
        _patched = true;
        new Harmony("downfall.compat.modtypes").Patch(
            AccessTools.PropertyGetter(typeof(ReflectionHelper), nameof(ReflectionHelper.ModTypes)),
            postfix: new HarmonyMethod(typeof(ModManagerCompat), nameof(AppendExtraTypes)));
        new Harmony("downfall.compat.modsubtypes").Patch(
            AccessTools.Method(typeof(ReflectionHelper), nameof(ReflectionHelper.GetSubtypesFromAssembly)),
            postfix: new HarmonyMethod(typeof(ModManagerCompat), nameof(AppendExtraSubtypes)));
    }

    private static void AppendExtraSubtypes(ref IEnumerable<Type> __result, Assembly assembly, Type parentType)
    {
        if (assembly != typeof(ModManagerCompat).Assembly) return;
        __result = __result.Concat(ExtraAssemblies.SelectMany(a => ReflectionHelper.GetSubtypesFromAssembly(a, parentType)));
    }

    private static void AppendExtraTypes(ref Type[] __result)
    {
        if (!ReferenceEquals(__result, _lastModTypes))
        {
            _lastModTypes = __result;
            _mergedModTypes = __result.Concat(ExtraAssemblies.SelectMany(a => a.GetTypes())).ToArray();
        }

        __result = _mergedModTypes;
    }
}
