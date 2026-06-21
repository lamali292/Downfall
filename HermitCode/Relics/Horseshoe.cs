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
[Obsolete]
public sealed class Horseshoe : HermitRelicModel
{
    public Horseshoe() : base(RelicRarity.Common, false)
    {
        WithTip<WeakPower>();
        WithTip<FrailPower>();
        WithTip<VulnerablePower>();
    }

    public override bool TryModifyPowerAmountReceived(
        PowerModel canonicalPower,
        Creature target,
        decimal amount,
        Creature? applier,
        out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        if (target != Owner.Creature)
            return false;

        if (amount <= 0m)
            return false;
        if (canonicalPower is not (WeakPower or FrailPower or VulnerablePower))
            return false;
        modifiedAmount = Math.Max(0m, amount - 1m);
        return true;
    }
}