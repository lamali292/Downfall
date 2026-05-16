using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using Champ.ChampCode.Stance;

namespace Champ.ChampCode.Powers;

public class ArenaMasteryDefensivePower : ChampPowerModel, IModifyFinisherBonus
{
    public int ModifyFinisherBonus(ChampStanceModel stanceModel, int baseAmount)
    {
        if (stanceModel.Owner.Creature != Owner) return baseAmount;
        if (stanceModel is ChampDefensiveStance or ChampUltimateStance)
            return baseAmount + Amount;
        return baseAmount;
    }
}