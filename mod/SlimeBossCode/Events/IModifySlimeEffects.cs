using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Events;

/// <summary>
/// Modifies a Slime's non-damage effect (e.g. Thorns, Soulburn) as <c>(base + additive) * multiplicative</c>.
/// Every additive modifier applies before any multiplicative one, so the result never depends on the order
/// powers were applied. Implement only the part you need.
/// </summary>
public interface IModifySlimeEffects
{
    /// <summary>The amount to add to <paramref name="slime"/>'s effect (0 for none).</summary>
    int ModifySlimeEffectsAdditive(SlimeModel slime) => 0;

    /// <summary>The factor to multiply <paramref name="slime"/>'s effect by (1 for none).</summary>
    int ModifySlimeEffectsMultiplicative(SlimeModel slime) => 1;
}
