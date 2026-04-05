using Downfall.Code.Core.Champ;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Events;

public interface IModifySkillBonus
{
    int ModifySkillBonus<TPower>(ChampStanceModel stance, int amount) where TPower : PowerModel;
}