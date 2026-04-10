using Downfall.Code.Abstract;
using Downfall.Code.Core.Champ;
using Downfall.Code.Events;

namespace Downfall.Code.Powers.Champ;

public class ArenaMasteryBerserkerPower : ChampPowerModel, IModifyFinisherBonus
{
    public int ModifyFinisherBonus(ChampStanceModel stanceModel, int baseAmount)
    {
        if (stanceModel.Owner.Creature != Owner) return baseAmount;
        if (stanceModel is BerserkerChampStance or UltimateChampStance)
            return baseAmount + Amount;
        return baseAmount;
    }
}