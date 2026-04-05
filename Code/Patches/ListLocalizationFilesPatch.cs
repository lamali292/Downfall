using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using FileAccess = Godot.FileAccess;

namespace Downfall.Code.Patches;

[HarmonyPatch(typeof(LocManager), "ListLocalizationFiles")]
public static class ListLocalizationFilesPatch
{
    private static readonly string[] ExtraTables = ["encode.json", "downfall.json"];

    public static void Postfix(ref IEnumerable<string> __result)
    {
        var result = __result;
        var enumerable = result as string[] ?? result.ToArray();
        __result = enumerable.Concat(ExtraTables.Where(f => !enumerable.Contains(f)));
    }
}

[HarmonyPatch(typeof(LocManager), "LoadTable")]
public static class LoadTablePatch
{
    public static bool Prefix(string path, ref Dictionary<string, string> __result)
    {
        if (FileAccess.FileExists(path)) return true;
        __result = new Dictionary<string, string>();
        return false;
    }
}