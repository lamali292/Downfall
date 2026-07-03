using System.Reflection;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace Downfall.DownfallCode.Compatibility;

public static class CompatibilityAnimation
{
    private const BindingFlags F = BindingFlags.Public | BindingFlags.Instance;

    private static readonly bool IsNewApi;
    private static readonly MethodInfo SetAnimation;    // SetAnimation(string, bool) — both versions
    private static readonly MethodInfo? GetCurrent;     // new: GetCurrent(int)
    private static readonly MethodInfo AddAnimation;    // old: AddAnimation(string) / new: AddAnimationTracked(string)
    private static readonly MethodInfo SetMixOnSetEntry;
    private static readonly MethodInfo SetMixOnAddEntry;

    static CompatibilityAnimation()
    {
        var t = typeof(MegaAnimationState);

        SetAnimation = t.GetMethod("SetAnimation", F, null, [typeof(string), typeof(bool)], null)
            ?? throw new MissingMethodException("MegaAnimationState.SetAnimation(string, bool) not found.");

        var tracked = t.GetMethod("AddAnimationTracked", F, null, [typeof(string)], null);
        IsNewApi = tracked != null;

        if (IsNewApi)
        {
            GetCurrent = t.GetMethod("GetCurrent", F, null, [typeof(int)], null)
                ?? throw new MissingMethodException("MegaAnimationState.GetCurrent(int) not found.");
            AddAnimation = tracked!;
        }
        else
        {
            AddAnimation = t.GetMethod("AddAnimation", F, null, [typeof(string)], null)
                ?? throw new MissingMethodException("MegaAnimationState.AddAnimation(string) not found.");
        }

        // Entry types may differ per version and per call path — resolve SetMixDuration off
        // the actual return types instead of hardcoding an entry type.
        var setEntryType = IsNewApi ? GetCurrent!.ReturnType : SetAnimation.ReturnType;
        SetMixOnSetEntry = setEntryType.GetMethod("SetMixDuration", [typeof(float)])
            ?? throw new MissingMethodException($"{setEntryType.Name}.SetMixDuration(float) not found.");
        SetMixOnAddEntry = AddAnimation.ReturnType.GetMethod("SetMixDuration", [typeof(float)])
            ?? throw new MissingMethodException($"{AddAnimation.ReturnType.Name}.SetMixDuration(float) not found.");
    }

    public static void SetAnimationWithMix(this MegaAnimationState animState,
        string anim, float mix, bool loop = true)
    {
        object? entry;
        if (IsNewApi)
        {
            SetAnimation.Invoke(animState, [anim, loop]);
            entry = GetCurrent!.Invoke(animState, [0]);
        }
        else
        {
            entry = SetAnimation.Invoke(animState, [anim, loop]);
        }

        try
        {
            if (entry != null)
                SetMixOnSetEntry.Invoke(entry, [mix]);
        }
        finally
        {
            (entry as IDisposable)?.Dispose();
        }
    }

    public static void QueueAnimation(this MegaAnimationState animState, string anim, float mix)
    {
        var entry = AddAnimation.Invoke(animState, [anim]);
        try
        {
            if (entry != null)
                SetMixOnAddEntry.Invoke(entry, [mix]);
        }
        finally
        {
            (entry as IDisposable)?.Dispose();
        }
    }
}