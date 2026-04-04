using Downfall.Code.Core.Champ;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Events;

public interface IModifySkillBonus
{
    int ModifySkillBonus<TPower>(PlayerChoiceContext ctx, StanceModel player, int amount)  where TPower : PowerModel;
}