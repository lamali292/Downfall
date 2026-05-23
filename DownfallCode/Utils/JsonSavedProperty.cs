using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using HarmonyLib;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Downfall.DownfallCode.Utils;

[AttributeUsage(AttributeTargets.Property)]
public class JsonSavedPropertyAttribute : Attribute;


[HarmonyPatch]
public static class JsonSavedPropertyPatches
{
    private const string Prefix = "json__";

    // Runs after FromInternal builds the SavedProperties —
    // adds JSON-serialized entries as strings with a reserved prefix
    [HarmonyPostfix]
    [HarmonyPatch(typeof(SavedProperties), nameof(SavedProperties.FromInternal))]
    public static void FromInternal_Postfix(object model, ref SavedProperties? __result)
    {
        var props = model.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(p => p.GetCustomAttribute<JsonSavedPropertyAttribute>() != null);

        foreach (var prop in props)
        {
            var value = prop.GetValue(model);
            if (value == null) continue;

            var json = JsonSerializer.Serialize(value);
            __result ??= new SavedProperties();
            __result.strings ??= new List<SavedProperties.SavedProperty<string>>();
            __result.strings.Add(new SavedProperties.SavedProperty<string>(Prefix + prop.Name, json));
        }
    }

    // Runs before FillInternal so the base game doesn't try to match our prefixed strings
    [HarmonyPrefix]
    [HarmonyPatch(typeof(SavedProperties), nameof(SavedProperties.FillInternal))]
    public static void FillInternal_Prefix(SavedProperties __instance, object model)
    {
        if (__instance.strings == null) return;

        var type = model.GetType();

        foreach (var entry in __instance.strings.Where(s => s.name.StartsWith(Prefix)))
        {
            var propName = entry.name[Prefix.Length..];
            var prop = type.GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop == null) continue;

            var deserialized = JsonSerializer.Deserialize(entry.value, prop.PropertyType);
            prop.SetValue(model, deserialized);
        }
    }
}

