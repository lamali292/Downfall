using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using Champ.ChampCode.Stance;

namespace Champ.ChampCode.Powers;

public class ArenaMasteryBerserkerPower : ChampPowerModel, IModifyFinisherBonus
{
    public int ModifyFinisherBonus(ChampStanceModel stanceModel, int baseAmount)
    {
        if (stanceModel.Owner.Creature != Owner) return baseAmount;
        if (stanceModel is ChampBerserkerStance or ChampUltimateStance)
            return baseAmount + Amount;
        return baseAmount;
    }
}