using System.Collections;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;

namespace Downfall.DownfallCode.Compatibility;

/// <summary>
/// Safety net for the net-ID table: after <see cref="ModelIdSerializationCache.Init" /> every model in
/// <see cref="ModelDb" /> must have a category and entry id, otherwise <c>ModelDb.InitIds</c> throws
/// "ModelId entry X could not be mapped to any net ID!" and the game never starts.
/// </summary>
/// <remarks>
/// Older game builds (V107) build that table by enumerating only the single <c>mod.assembly</c> of each mod
/// via <c>ReflectionHelper.GetSubtypesFromAssembly</c>. Our internal submods are separate assemblies that
/// <see cref="ModManagerCompat.AssociateAssemblyWithMod" /> can only add by postfixing that tiny method, and the
/// JIT may inline it into <c>Init</c>, in which case the postfix silently never runs: <see cref="ModelDb" /> knows
/// the submod's models (it reads <c>ReflectionHelper.ModTypes</c>) but the table does not.
/// Missing names are only appended, never reordered, so every id the game already handed out stays valid.
/// Rebuilds nothing else, so on a game build that already maps everything this is a no-op.
/// </remarks>
[HarmonyPatch(typeof(ModelIdSerializationCache), nameof(ModelIdSerializationCache.Init))]
internal static class ModelIdCacheCompletionPatch
{
    private const BindingFlags AllStatic = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    private static readonly Type CacheType = typeof(ModelIdSerializationCache);

    [HarmonyPostfix]
    private static void Postfix()
    {
        try
        {
            var added = EnsureAllModelsMapped();
            if (added > 0)
                DownfallMainFile.Logger.Warn(
                    $"[ModelIdCache] {added} model(s) were missing from the net-ID table and have been appended.");
        }
        catch (Exception e)
        {
            DownfallMainFile.Logger.Error($"[ModelIdCache] Could not complete the net-ID table: {e}");
        }
    }

    /// <returns>how many models had a category or entry name that was not in the table.</returns>
    internal static int EnsureAllModelsMapped()
    {
        var categoryMap = GetField<Dictionary<string, int>>("_categoryNameToNetIdMap");
        var categoryList = GetField<List<string>>("_netIdToCategoryNameMap");
        var entryMap = GetField<Dictionary<string, int>>("_entryNameToNetIdMap");
        var entryList = GetField<List<string>>("_netIdToEntryNameMap");
        if (categoryMap == null || categoryList == null || entryMap == null || entryList == null)
        {
            DownfallMainFile.Logger.Warn("[ModelIdCache] Net-ID table internals not found; skipping the completeness check.");
            return 0;
        }

        // ModelDb.All does not exist on V107, so read the registry itself (RitsuLib does the same).
        if (AccessTools.DeclaredField(typeof(ModelDb), "_contentById")?.GetValue(null) is not IDictionary content)
        {
            DownfallMainFile.Logger.Warn("[ModelIdCache] ModelDb registry not found; skipping the completeness check.");
            return 0;
        }

        var missing = content.Values.OfType<AbstractModel>()
            .Select(static model => model.GetType())
            .Distinct()
            .Select(static type => (type, id: ModelDb.GetId(type)))
            .Where(p => !categoryMap.ContainsKey(p.id.Category) || !entryMap.ContainsKey(p.id.Entry))
            .OrderBy(static p => p.id.Category, StringComparer.Ordinal)
            .ThenBy(static p => p.id.Entry, StringComparer.Ordinal)
            .ToList();
        if (missing.Count == 0) return 0;

        foreach (var (_, id) in missing)
        {
            Append(id.Category, categoryMap, categoryList);
            Append(id.Entry, entryMap, entryList);
        }

        SetBitSize(nameof(ModelIdSerializationCache.CategoryIdBitSize), categoryList.Count);
        SetBitSize(nameof(ModelIdSerializationCache.EntryIdBitSize), entryList.Count);

        // Newer builds also cache each type's [SavedProperty] list here; a type the table never saw has none.
        var cacheProperties = CacheType.GetMethod("CachePropertiesForType", AllStatic);
        if (cacheProperties != null)
        {
            foreach (var (type, _) in missing)
                cacheProperties.Invoke(null, [type, null, null]);

            var propertyList = GetField<List<string>>("_netIdToPropertyNameMap");
            if (propertyList != null)
                SetBitSize("PropertyIdBitSize", propertyList.Count);
        }

        return missing.Count;
    }

    private static void Append(string name, Dictionary<string, int> map, List<string> list)
    {
        if (map.ContainsKey(name)) return;
        map[name] = list.Count;
        list.Add(name);
    }

    // Same formula the game uses after its own Init.
    private static void SetBitSize(string propertyName, int count)
    {
        var bits = count <= 1 ? 0 : (int)Math.Ceiling(Math.Log2(count));
        CacheType.GetProperty(propertyName, AllStatic)?.GetSetMethod(true)?.Invoke(null, [bits]);
    }

    private static T? GetField<T>(string name) where T : class
    {
        return AccessTools.DeclaredField(CacheType, name)?.GetValue(null) as T;
    }
}
