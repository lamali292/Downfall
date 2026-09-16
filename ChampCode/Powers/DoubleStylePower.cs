using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Powers;

public class DoubleStylePower : ChampPowerModel, IModifySkillBonus
{
    public DoubleStylePower()
    {
        WithTip<CounterPower>();
        WithTip<VigorPower>();
    }
    


    public int ModifySkillBonus<TPower>(ChampStanceModel stance, int amount)
        where TPower : PowerModel
    {
        
        if ((typeof(TPower) == typeof(VigorPower) || typeof(TPower) ==  typeof(CounterPower)) && stance.Owner.Creature == Owner) return amount+ Amount;
        return amount;
    }
}