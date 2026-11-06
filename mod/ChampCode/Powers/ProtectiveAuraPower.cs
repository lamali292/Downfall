using Champ.ChampCode.Core;
using Champ.ChampCode.Events;

namespace Champ.ChampCode.Powers;

public class ProtectiveAuraPower : ChampPowerModel, IModifyDefensiveFinisherBonus
{
    public int ModifyDefensiveFinisherBonus(ChampStanceModel stanceModel, int baseAmount)
    {
        return stanceModel.Owner.Creature == Owner ? baseAmount + Amount : baseAmount;
    }
}