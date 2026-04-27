using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Powers;

public class ChainLashPower : ChampPowerModel, IModifySkillBonus
{
    public int ModifySkillBonus<TPower>(ChampStanceModel stance, int amount) where TPower : PowerModel
    {
        if (stance.Owner.Creature == Owner) return amount + Amount;
        return amount;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side) return;
        await PowerCmd.Remove(this);
    }
}