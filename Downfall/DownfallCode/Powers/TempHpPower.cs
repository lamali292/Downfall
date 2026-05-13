using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.DownfallCode.Powers;

public class TempHpPower : DownfallPowerModel
{
    private decimal _absorbed;
    
    public override decimal ModifyHpLostBeforeOsty(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner) return amount;
        _absorbed = Math.Min(Amount, (int)amount);
        return amount - _absorbed;
    }


    public override  Task AfterModifyingHpLostBeforeOsty()
    {
        return PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -_absorbed,  null,  null, true);
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext ctx, Creature target, DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner) return;
        if (Amount <= 0)
            await PowerCmd.Remove(this);
    }
}