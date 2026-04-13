using Downfall.Code.Core.Champ;

namespace Downfall.Code.Events;

public interface IModifyFinisherBonus
{
    int ModifyFinisherBonus(ChampStanceModel stanceModel, int baseAmount);
}