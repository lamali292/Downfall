using System.Collections.Generic;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using Downfall.DownfallCode.Localization;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(ModManager), nameof(ModManager.GetModdedLocTables))]
static class GetModdedLocTablesPatch
{
    static IEnumerable<string> Postfix(IEnumerable<string> values, string language, string file)
    {
        foreach (var path in values)
            yield return path;

        foreach (var id in BundledSubmodLocRegistry.Ids)
        {
            var path = $"res://{id}/localization/{language}/{file}";
            if (ResourceLoader.Exists(path))
                yield return path;
        }
    }
}