using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Events;

/// <summary>
/// Fires once per card produced by CardCmd.Transform. The argument is the *replacement*.
/// The game enforces replacement.Owner == original.Owner, so owner-gated effects
/// (like Straight Razor) can safely gate on replacement.Owner.
/// </summary>
public interface IAfterCardTransformed
{
    Task AfterCardTransformed(CardModel replacement);
}