using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     Whenever you gain Weak, Frail or Vulnerable, gain 1 less.
///     Uses the built-in TryModifyPowerAmountReceived hook.
/// </summary>
public sealed class Horseshoe : HermitRelicModel
{
    public Horseshoe() : base(RelicRarity.Common)
    {
        this.WithTip<WeakPower>();
        this.WithTip<FrailPower>();
        this.WithTip<VulnerablePower>();
    }

    public override bool TryModifyPowerAmountReceived(
        PowerModel canonicalPower,
        Creature target,
        decimal amount,
        Creature? applier,
        out decimal modifiedAmount)
    {
        modifiedAmount = amount;

        // Only reduce debuffs being applied to the relic owner
        if (target != Owner?.Creature)
            return false;

        // Only trigger for positive amounts (application, not removal)
        if (amount <= 0m)
            return false;

        // Only intercept Weak, Frail, or Vulnerable
        if (canonicalPower is not (WeakPower or FrailPower or VulnerablePower))
            return false;

        // Reduce by 1, minimum 0
        modifiedAmount = Math.Max(0m, amount - 1m);
        return true;
    }
}