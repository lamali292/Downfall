using Champ.ChampCode.Core;

namespace Champ.ChampCode.Events;

public interface IModifyFinisherBonus
{
    int ModifyFinisherBonus(ChampStanceModel stanceModel, int baseAmount);
}