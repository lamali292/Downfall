using System.Reflection;
using System.Text.Json;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Saves;

namespace Downfall.DownfallCode.Utils.ModdedSaves;

[AttributeUsage(AttributeTargets.Field)]
public class ModSaveAttribute : Attribute;

public static class ModSaveHelper
{
    private static readonly Dictionary<string, FieldInfo> SaveFieldCache = new();

    public static IEnumerable<Mod> GetActiveMods()
    {
        return ModManager.GetLoadedMods();
    }

    public static string GetModId(Mod mod)
    {
        return mod.manifest?.id ?? "UnknownMod";
    }

    public static string GetModPath(string vanillaPath, string modId)
    {
        var directory = Path.GetDirectoryName(vanillaPath) ?? "";
        var fileName = Path.GetFileName(vanillaPath);
        return Path.Combine(directory, "mods", modId, fileName).Replace("\\", "/");
    }

    private static FieldInfo? GetSaveField(Mod mod)
    {
        var modId = GetModId(mod);
        if (SaveFieldCache.TryGetValue(modId, out var cachedField)) return cachedField;
        var field = mod.assembly?.GetTypes()
            .SelectMany(t => t.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            .FirstOrDefault(f => f.GetCustomAttribute<ModSaveAttribute>() != null);

        if (field != null) SaveFieldCache[modId] = field;
        return field;
    }

    public static string? GetModDataToSave(Mod mod)
    {
        var field = GetSaveField(mod);
        if (field == null) return null;
        var liveData = field.GetValue(null);
        if (liveData == null) return null;
        return JsonSerializer.Serialize(liveData, field.FieldType, new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true
        });
    }

    public static void LoadDataIntoMod(Mod mod, string json)
    {
        var field = GetSaveField(mod);
        if (field == null) return;

        try
        {
            var loadedData = JsonSerializer.Deserialize(json, field.FieldType, new JsonSerializerOptions
            {
                IncludeFields = true
            });

            if (loadedData == null) return;
            field.SetValue(null, loadedData);
        }
        catch (Exception ex)
        {
            DownfallMainFile.Logger.Error($"Load error for {mod.manifest?.id}: {ex.Message}");
        }
    }
}

[HarmonyPatch(typeof(CloudSaveStore))]
public static class ModdedSaveStorePatch
{
    private static bool _isInternal;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(CloudSaveStore.WriteFile), typeof(string), typeof(string))]
    public static void PostfixSyncString(CloudSaveStore __instance, string path)
    {
        ProcessTrigger(__instance, path);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(CloudSaveStore.WriteFile), typeof(string), typeof(byte[]))]
    public static void PostfixSyncBytes(CloudSaveStore __instance, string path)
    {
        ProcessTrigger(__instance, path);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(CloudSaveStore.WriteFileAsync), typeof(string), typeof(string))]
    public static void PostfixAsyncString(CloudSaveStore __instance, string path)
    {
        ProcessTrigger(__instance, path);
    }


    [HarmonyPostfix]
    [HarmonyPatch(nameof(CloudSaveStore.WriteFileAsync), typeof(string), typeof(byte[]))]
    public static void PostfixAsyncBytes(CloudSaveStore __instance, string path)
    {
        ProcessTrigger(__instance, path);
    }

    private static void ProcessTrigger(CloudSaveStore __instance, string path)
    {
        var isRunSave = path.EndsWith("current_run.save") || path.EndsWith("current_run_mp.save");
        if (_isInternal || !isRunSave) return;

        _isInternal = true;
        try
        {
            foreach (var mod in ModSaveHelper.GetActiveMods())
            {
                var modId = ModSaveHelper.GetModId(mod);
                var modPath = ModSaveHelper.GetModPath(path, modId);
                var modData = ModSaveHelper.GetModDataToSave(mod);
                if (!string.IsNullOrEmpty(modData) && !modData.Equals("{}")) __instance.WriteFile(modPath, modData);
            }
        }
        finally
        {
            _isInternal = false;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(CloudSaveStore.ReadFile), typeof(string))]
    public static void PostfixReadFile(CloudSaveStore __instance, string path, ref string __result)
    {
        if (IsInvalidRead(path, __result)) return;
        ProcessRead(__instance, path);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(CloudSaveStore.ReadFileAsync), typeof(string))]
    public static async Task<string?> PostfixReadFileAsync(Task<string?> __result, CloudSaveStore __instance,
        string path)
    {
        var content = await __result;
        if (!IsInvalidRead(path, content)) ProcessRead(__instance, path);
        return content;
    }

    private static void ProcessRead(CloudSaveStore store, string vanillaPath)
    {
        foreach (var mod in ModSaveHelper.GetActiveMods())
        {
            var modPath = ModSaveHelper.GetModPath(vanillaPath, ModSaveHelper.GetModId(mod));

            if (!store.FileExists(modPath)) continue;
            var modJson = store.ReadFile(modPath);
            if (!string.IsNullOrEmpty(modJson)) ModSaveHelper.LoadDataIntoMod(mod, modJson);
        }
    }

    private static bool IsInvalidRead(string path, string? content)
    {
        return string.IsNullOrEmpty(content) ||
               (!path.EndsWith("current_run.save") && !path.EndsWith("current_run_mp.save"));
    }
}

[HarmonyPatch(typeof(SaveManager), "EnumerateCloudSyncTasks")]
public static class UniversalModSyncPatch
{
    public static IEnumerable<Task> Postfix(IEnumerable<Task> __result, CloudSaveStore cloudStore)
    {
        foreach (var task in __result) yield return task;

        for (var i = 1; i <= 3; i++)
        {
            var profileModDir = UserDataPathProvider.GetProfileDir(i) + "/saves/mods";

            // This is truly generic: it asks Steam what directories exist
            foreach (var modFolder in cloudStore.CloudStore.GetDirectoriesInDirectory(profileModDir))
            {
                yield return cloudStore.SyncCloudToLocal($"{profileModDir}/{modFolder}/current_run.save");
                yield return cloudStore.SyncCloudToLocal($"{profileModDir}/{modFolder}/current_run_mp.save");
            }
        }
    }
}

[HarmonyPatch(typeof(CloudSaveStore), "DeleteFile")]
public static class UniversalModDeletePatch
{
    public static void Postfix(CloudSaveStore __instance, string path)
    {
        var isTargetFile = path.EndsWith("current_run.save") ||
                           path.EndsWith("current_run_mp.save") ||
                           path.EndsWith(".save.backup");
        if (!isTargetFile) return;

        foreach (var mod in ModSaveHelper.GetActiveMods())
        {
            var modId = ModSaveHelper.GetModId(mod);
            var modPath = ModSaveHelper.GetModPath(path, modId);

            if (__instance.FileExists(modPath)) __instance.DeleteFile(modPath);
        }
    }
}