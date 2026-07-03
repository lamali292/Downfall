using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace Downfall.DownfallCode.Compatibility;

public static class CompatibilityAnimation
{
    public static void SetAnimationWithMix(this MegaAnimationState animState,
        string anim, float mix, bool loop = true)
    {
#if V107
        animState.SetAnimation(anim, loop)
            ?.SetMixDuration(mix);
#else
        animState.SetAnimation(anim, loop);
        using var entry = animState.GetCurrent(0);
        entry?.SetMixDuration(mix);
#endif
    }

    public static void QueueAnimation(this MegaAnimationState animState,
        string anim, float mix)
    {
#if V107
        animState.AddAnimation(anim)
            .SetMixDuration(mix);
#else
        using var entry = animState.AddAnimationTracked(anim);
        entry.SetMixDuration(mix);
#endif
    }
}