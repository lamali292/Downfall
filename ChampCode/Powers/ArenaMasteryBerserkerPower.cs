using Champ.ChampCode.Core;
using Champ.ChampCode.Events;

namespace Champ.ChampCode.Powers;

public class ArenaMasteryBerserkerPower : ChampPowerModel, IModifyBerserkerFinisherBonus
{
    public int ModifyBerserkerFinisherBonus(ChampStanceModel stanceModel, int baseAmount)
    {
        return stanceModel.Owner.Creature == Owner ? baseAmount + Amount : baseAmount;
    }
}