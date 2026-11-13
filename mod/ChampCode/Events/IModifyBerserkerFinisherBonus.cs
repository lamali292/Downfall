using Champ.ChampCode.Core;

namespace Champ.ChampCode.Events;

public interface IModifyBerserkerFinisherBonus
{
    int ModifyBerserkerFinisherBonus(ChampStanceModel stanceModel, int baseAmount);
}