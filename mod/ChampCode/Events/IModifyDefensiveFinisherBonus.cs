using Champ.ChampCode.Core;

namespace Champ.ChampCode.Events;

public interface IModifyDefensiveFinisherBonus
{
    int ModifyDefensiveFinisherBonus(ChampStanceModel stanceModel, int baseAmount);
}