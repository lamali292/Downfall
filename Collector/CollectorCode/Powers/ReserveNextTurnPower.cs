using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Powers;

public class ReserveNextTurnPower : CollectorPowerModel
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (Owner != player.Creature) return;
        CardResourceRegistry.Get<CollectorEnergy>()?.Gain(player, Amount);
        await PowerCmd.Remove(this);
    }
}