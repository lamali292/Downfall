using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Powers;

public class BronzeBramblesPower : GuardianPowerModel
{
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext ctx, PowerModel power, decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power.Owner != Owner) return;

        var oldAmount = power.Amount - amount;
        var oldType = power.GetTypeForAmount(oldAmount);
        var newType = power.TypeForCurrentAmount;

        var worseOff =
            (oldType == PowerType.Buff && newType == PowerType.Debuff)            
            || (newType == PowerType.Debuff && Math.Abs(power.Amount) > Math.Abs(oldAmount))  
            || (oldType != PowerType.Debuff && newType == PowerType.Buff && Math.Abs(power.Amount) < Math.Abs(oldAmount)); 
        if (!worseOff) return;

        await PowerCmd.Apply<ThornsPower>(ctx, Owner, Amount, applier, null);
        Flash();
    }
}