using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Downfall.DownfallCode.Compatibility;

/// <summary>
///     A <see cref="DynamicVar"/> whose value is computed on demand (not stored in <c>BaseValue</c>/<c>PreviewValue</c>).
///     Older builds convert vars through the protected virtual <c>GetBaseValueForIConvertible</c>, which newer
///     builds removed (they read the preview value instead). Implement this instead of overriding that method; the
///     patch below applies it on the builds that still have the virtual.
/// </summary>
public interface IConvertibleValueSource
{
    decimal GetConvertibleValue();
}

[HarmonyPatch]
internal static class DynamicVarConvertibleValuePatch
{
    private static bool Prepare() => TargetMethod() != null;

    private static MethodBase? TargetMethod() => AccessTools.Method(typeof(DynamicVar), "GetBaseValueForIConvertible");

    [HarmonyPostfix]
    private static void Postfix(DynamicVar __instance, ref decimal __result)
    {
        if (__instance is IConvertibleValueSource source) __result = source.GetConvertibleValue();
    }
}
