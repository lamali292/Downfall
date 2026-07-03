using System.Reflection;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace Downfall.DownfallCode.Compatibility;

public static class CompatibilityAnimation
{
    private const BindingFlags F = BindingFlags.Public | BindingFlags.Instance;

    private static readonly MethodInfo SetAnimationM;   // SetAnimation(string, [bool], [...])
    private static readonly MethodInfo? GetCurrentM;    // GetCurrent(int), if present
    private static readonly MethodInfo AddAnimationM;   // AddAnimationTracked(string) or AddAnimation(string, [...])

    static CompatibilityAnimation()
    {
        var t = typeof(MegaAnimationState);

        SetAnimationM = FindByName(t, "SetAnimation", typeof(string), typeof(bool))
            ?? throw new MissingMethodException("MegaAnimationState.SetAnimation(string, bool, ...) not found.");

        GetCurrentM = FindByName(t, "GetCurrent", typeof(int));

        AddAnimationM = FindByName(t, "AddAnimationTracked", typeof(string))
            ?? FindByName(t, "AddAnimation", typeof(string))
            ?? throw new MissingMethodException("MegaAnimationState.AddAnimation(Tracked)(string, ...) not found.");
    }

    /// <summary>
    /// Finds a method whose leading parameters match <paramref name="leading"/> exactly and
    /// whose remaining parameters are all optional. Handles C# default parameters, which are
    /// invisible to exact-signature GetMethod lookups.
    /// </summary>
    private static MethodInfo? FindByName(Type type, string name, params Type[] leading)
    {
        return type.GetMethods(F)
            .Where(m => m.Name == name)
            .Where(m =>
            {
                var ps = m.GetParameters();
                if (ps.Length < leading.Length) return false;
                for (var i = 0; i < leading.Length; i++)
                    if (ps[i].ParameterType != leading[i]) return false;
                for (var i = leading.Length; i < ps.Length; i++)
                    if (!ps[i].IsOptional) return false;
                return true;
            })
            .OrderBy(m => m.GetParameters().Length) // prefer the tightest match
            .FirstOrDefault();
    }

    /// <summary>Invokes, padding unsupplied trailing optional parameters with their defaults.</summary>
    private static object? Call(MethodInfo m, object target, params object?[] args)
    {
        var ps = m.GetParameters();
        if (ps.Length == args.Length)
            return m.Invoke(target, args);

        var full = new object?[ps.Length];
        Array.Copy(args, full, args.Length);
        for (var i = args.Length; i < ps.Length; i++)
            full[i] = ps[i].DefaultValue;
        return m.Invoke(target, full);
    }

    private static void SetMixDuration(object entry, float mix)
    {
        var m = FindByName(entry.GetType(), "SetMixDuration", typeof(float))
            ?? throw new MissingMethodException($"{entry.GetType().Name}.SetMixDuration(float) not found.");
        Call(m, entry, mix);
    }

    public static void SetAnimationWithMix(this MegaAnimationState animState,
        string anim, float mix, bool loop = true)
    {
        // Some versions return the entry directly; others return void and expose GetCurrent(0).
        var entry = Call(SetAnimationM, animState, anim, loop);
        if (entry is null && GetCurrentM != null)
            entry = Call(GetCurrentM, animState, 0);

        try
        {
            if (entry != null)
                SetMixDuration(entry, mix);
        }
        finally
        {
            (entry as IDisposable)?.Dispose();
        }
    }

    public static void QueueAnimation(this MegaAnimationState animState, string anim, float mix)
    {
        var entry = Call(AddAnimationM, animState, anim);
        try
        {
            if (entry != null)
                SetMixDuration(entry, mix);
        }
        finally
        {
            (entry as IDisposable)?.Dispose();
        }
    }
}