using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Powers;

public class BerserkerStylePower : ChampPowerModel, IModifySkillBonus
{
    public BerserkerStylePower()
    {
        WithTip(typeof(VigorPower));
    }


    public int ModifySkillBonus<TPower>(ChampStanceModel stance, int amount)
        where TPower : PowerModel
    {
        if (typeof(TPower) != typeof(VigorPower) || stance.Owner.Creature != Owner) return amount;
        return amount + Amount;
    }
}