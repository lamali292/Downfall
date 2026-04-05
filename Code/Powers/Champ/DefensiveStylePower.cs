using Downfall.Code.Abstract;
using Downfall.Code.Core.Champ;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Powers.Champ;

public class DefensiveStylePower : ChampPowerModel, IModifySkillBonus
{
    public DefensiveStylePower()
    {
        WithTip(typeof(CounterPower));
    }

    public int ModifySkillBonus<TPower>(ChampStanceModel stance, int amount)
        where TPower : PowerModel
    {
        if (typeof(TPower) != typeof(CounterPower) || stance.Owner.Creature != Owner) return amount;
        return amount + Amount;
    }
}