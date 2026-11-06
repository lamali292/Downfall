using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Events;

public interface IModifySkillBonus
{
    int ModifySkillBonus<TPower>(ChampStanceModel stance, int amount) where TPower : PowerModel;
}