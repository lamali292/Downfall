using Downfall.Code.Abstract;
using Downfall.Code.Core.Collector;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Collector;

public class ReserveNextTurnPower : CollectorPowerModel
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        CollectorEnergy.Gain(player, Amount);
        await PowerCmd.Remove(this);
    }
}