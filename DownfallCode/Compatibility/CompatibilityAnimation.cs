using System.Reflection;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace Downfall.DownfallCode.Compatibility;

public static class CompatibilityAnimation
{
    private const BindingFlags F = BindingFlags.Public | BindingFlags.Instance;

    // Nullable + lazy: a failed probe must not poison the type (a throwing static ctor
    // makes EVERY later call rethrow TypeInitializationException — the opposite of "never crash").
    private static MethodInfo? _setAnimationM;
    private static MethodInfo? _getCurrentM;
    private static MethodInfo? _addAnimationM;
    private static MethodInfo? _addEmptyAnimationM;
    private static bool _initialized;
    private static bool _initFailed;

    private static bool EnsureInitialized()
    {
        if (_initialized) return !_initFailed;
        _initialized = true;
        try
        {
            var t = typeof(MegaAnimationState);
            _setAnimationM = FindByName(t, "SetAnimation", typeof(string), typeof(bool));
            _getCurrentM = FindByName(t, "GetCurrent", typeof(int));
            _addAnimationM = FindByName(t, "AddAnimationTracked", typeof(string))
                          ?? FindByName(t, "AddAnimation", typeof(string));
            _addEmptyAnimationM = FindByName(t, "AddEmptyAnimation");

            if (_setAnimationM == null)
                DownfallMainFile.Logger.Warn("CompatibilityAnimation: SetAnimation not found — animations will be skipped.");
            if (_addAnimationM == null)
                DownfallMainFile.Logger.Warn("CompatibilityAnimation: AddAnimation(Tracked) not found — queued animations will be skipped.");

            _initFailed = _setAnimationM == null && _addAnimationM == null;
        }
        catch (Exception ex)
        {
            _initFailed = true;
            DownfallMainFile.Logger.Warn($"CompatibilityAnimation: init failed, animations disabled. {ex.Message}");
        }
        return !_initFailed;
    }

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
            .OrderBy(m => m.GetParameters().Length)
            .FirstOrDefault();
    }

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

    private static void TrySetMixDuration(object entry, float mix)
    {
        var m = FindByName(entry.GetType(), "SetMixDuration", typeof(float));
        if (m == null)
        {
            LogOnce("SetMixDuration", $"{entry.GetType().Name}.SetMixDuration(float) not found — mix ignored.");
            return;
        }
        Call(m, entry, mix);
    }

    // Log each distinct failure once, not every frame.
    private static readonly HashSet<string> LoggedFailures = [];
    private static void LogOnce(string key, string message)
    {
        if (LoggedFailures.Add(key))
            DownfallMainFile.Logger.Warn($"CompatibilityAnimation: {message}");
    }

    // ------- pass-throughs: ALWAYS use these instead of calling MegaAnimationState directly.
    // A direct call bakes one version's signature into IL and JIT-crashes the entire
    // containing method on the other version. -------

    /// <summary>Version-safe SetAnimation (no mix). 107 returns an entry (disposed), 108 returns void.</summary>
    public static void SetAnimationCompat(this MegaAnimationState animState, string anim, bool loop = true)
    {
        if (!EnsureInitialized() || _setAnimationM == null) return;
        object? entry = null;
        try
        {
            entry = Call(_setAnimationM, animState, anim, loop);
        }
        catch (Exception ex)
        {
            LogOnce($"SetAnimationCompat:{ex.GetType().Name}", $"SetAnimation failed: {ex.InnerException?.Message ?? ex.Message}");
        }
        finally
        {
            try { (entry as IDisposable)?.Dispose(); } catch { }
        }
    }

    /// <summary>Version-safe fire-and-forget AddAnimation (no mix).</summary>
    public static void AddAnimationCompat(this MegaAnimationState animState, string anim)
    {
        if (!EnsureInitialized() || _addAnimationM == null) return;
        object? entry = null;
        try
        {
            entry = Call(_addAnimationM, animState, anim);
        }
        catch (Exception ex)
        {
            LogOnce($"AddAnimationCompat:{ex.GetType().Name}", $"AddAnimation failed: {ex.InnerException?.Message ?? ex.Message}");
        }
        finally
        {
            try { (entry as IDisposable)?.Dispose(); } catch { }
        }
    }

    /// <summary>Version-safe AddEmptyAnimation (fades the track out). Return type differs across versions.</summary>
    public static void AddEmptyAnimationCompat(this MegaAnimationState animState)
    {
        if (!EnsureInitialized() || _addEmptyAnimationM == null) return;
        object? entry = null;
        try
        {
            entry = Call(_addEmptyAnimationM, animState);
        }
        catch (Exception ex)
        {
            LogOnce($"AddEmptyAnimationCompat:{ex.GetType().Name}", $"AddEmptyAnimation failed: {ex.InnerException?.Message ?? ex.Message}");
        }
        finally
        {
            try { (entry as IDisposable)?.Dispose(); } catch { }
        }
    }

    // ------- mix variants -------

    public static void SetAnimationWithMix(this MegaAnimationState animState,
        string anim, float mix, bool loop = true)
    {
        if (!EnsureInitialized() || _setAnimationM == null) return;
        object? entry = null;
        try
        {
            entry = Call(_setAnimationM, animState, anim, loop);
            if (entry is null && _getCurrentM != null)
                entry = Call(_getCurrentM, animState, 0);

            if (entry != null)
                TrySetMixDuration(entry, mix);
        }
        catch (Exception ex)
        {
            LogOnce($"SetAnimationWithMix:{ex.GetType().Name}", $"SetAnimationWithMix failed: {ex.InnerException?.Message ?? ex.Message}");
        }
        finally
        {
            try { (entry as IDisposable)?.Dispose(); } catch { /* native teardown failure — nothing to do */ }
        }
    }

    public static void QueueAnimation(this MegaAnimationState animState, string anim, float mix)
    {
        if (!EnsureInitialized() || _addAnimationM == null) return;
        object? entry = null;
        try
        {
            entry = Call(_addAnimationM, animState, anim);
            if (entry != null)
                TrySetMixDuration(entry, mix);
        }
        catch (Exception ex)
        {
            LogOnce($"QueueAnimation:{ex.GetType().Name}", $"QueueAnimation failed: {ex.InnerException?.Message ?? ex.Message}");
        }
        finally
        {
            try { (entry as IDisposable)?.Dispose(); } catch { }
        }
    }
    
    /// <summary>
    /// Version-safe: plays an animation and randomizes its start time within the clip
    /// (used to de-sync idle loops). 107: uses SetAnimation's returned entry.
    /// 108: SetAnimation returns void → fetches the entry via GetCurrent(0).
    /// </summary>
    public static void SetAnimationRandomStart(this MegaAnimationState animState,
        string anim, bool loop, float normalizedTime)
    {
        if (!EnsureInitialized() || _setAnimationM == null) return;
        object? entry = null;
        try
        {
            entry = Call(_setAnimationM, animState, anim, loop);
            if (entry is null && _getCurrentM != null)
                entry = Call(_getCurrentM, animState, 0);
            if (entry == null) return;

            var getEnd = FindByName(entry.GetType(), "GetAnimationEnd");
            var setTime = FindByName(entry.GetType(), "SetTrackTime", typeof(float));
            if (getEnd == null || setTime == null)
            {
                LogOnce("RandomStart", $"{entry.GetType().Name}: GetAnimationEnd/SetTrackTime not found — random start skipped.");
                return;
            }

            var end = Convert.ToSingle(Call(getEnd, entry));
            Call(setTime, entry, end * normalizedTime);
        }
        catch (Exception ex)
        {
            LogOnce($"SetAnimationRandomStart:{ex.GetType().Name}",
                $"SetAnimationRandomStart failed: {ex.InnerException?.Message ?? ex.Message}");
        }
        finally
        {
            try { (entry as IDisposable)?.Dispose(); } catch { }
        }
    }
}