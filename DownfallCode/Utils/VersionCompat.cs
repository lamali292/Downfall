
using System;
using System.Reflection;
using Godot;

namespace Downfall.DownfallCode.Utils;


/// <summary>
/// Helpers for calling into types/members that may not exist on older
/// game versions. Resolves via reflection once, caches the result,
/// and no-ops (or returns a fallback) if the member isn't present.
/// </summary>
public static class VersionCompat
{
    private const string DefaultAssembly = "sts2";

    public static Type? FindType(string fullyQualifiedName, string assembly = DefaultAssembly)
    {
        return Type.GetType($"{fullyQualifiedName}, {assembly}");
    }

    public static MethodInfo? FindStaticMethod(Type? type, string methodName,
        BindingFlags flags = BindingFlags.Public | BindingFlags.Static)
    {
        return type?.GetMethod(methodName, flags);
    }

    public static FieldInfo? FindStaticField(Type? type, string fieldName,
        BindingFlags flags = BindingFlags.Public | BindingFlags.Static)
    {
        return type?.GetField(fieldName, flags);
    }

    /// <summary>
    /// Invokes a cached static method if it exists, returns default(T) otherwise.
    /// </summary>
    public static T? InvokeStatic<T>(MethodInfo? method, params object?[] args)
    {
        if (method == null) return default;
        return (T?)method.Invoke(null, args);
    }

    /// <summary>
    /// Reads a cached static field's value if it exists, returns fallback otherwise.
    /// </summary>
    public static T GetStaticField<T>(FieldInfo? field, T fallback)
    {
        if (field == null) return fallback;
        var value = field.GetValue(null);
        return value is T typed ? typed : fallback;
    }
}