using Downfall.Code.Abstract;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Champ;

public class ParryingPower : ChampPowerModel, IModifyCounterStrikeCount
{
    public int ModifyCounterStrikeCount(Player player, int amount)
    {
        if (player.Creature == Owner) return amount + Amount;
        return amount;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side) return;
        await PowerCmd.Remove(this);
    }
}