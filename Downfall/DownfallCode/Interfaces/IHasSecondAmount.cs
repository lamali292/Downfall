using Downfall.DownfallCode.Patches;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Interfaces;


/// <summary>
/// Exposes a secondary display amount alongside the primary power amount.
/// Implement this interface on any <c>PowerModel</c> that needs to show a
/// second numeric or textual value in the UI (e.g. a counter or cooldown).
/// </summary>
/// <remarks>
/// When the value changes, call <see cref="PowerExtensions.InvokeSecondAmountChanged"/>
/// to trigger a UI refresh.
/// </remarks>
public interface IHasSecondAmount
{
    /// <summary>
    /// Gets the secondary amount as a formatted string for UI display.
    /// </summary>
    /// <returns>
    /// A string representing the secondary value.
    /// </returns>
    string GetSecondAmount();
}





/// <summary>
/// Extension methods for <see cref="IHasSecondAmount"/> powers.
/// Connects them to their <see cref="NPower"/> display node,
/// allowing powers to trigger a label refresh without access to the node directly.
/// </summary>
public static class PowerExtensions
{
 
    /// <summary>
    /// Triggers a UI refresh of the second amount label for this power.
    /// Call this whenever the value returned by <see cref="IHasSecondAmount.GetSecondAmount"/> changes.
    /// </summary>
    /// <remarks>
    /// Typical usage inside a power:
    /// <code>
    /// this.InvokeSecondAmountChanged();
    /// </code>
    /// </remarks>
    public static void InvokeSecondAmountChanged(this IHasSecondAmount power)
    {
        if (SecondAmountRegistry.RefreshActions.TryGetValue(power, out var refresh))
            refresh?.Invoke();
    }
}


