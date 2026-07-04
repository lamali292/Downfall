using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Powers;

public sealed class RuggedPower : HermitPowerModel
{
    public override decimal ModifyHpLostBeforeOsty(Creature target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || Amount <= 0 || amount <= 0m || !props.IsPoweredAttack())
            return amount;
        SetAmount(Amount - 1);
        return Math.Min(amount, 2m);
    }
}