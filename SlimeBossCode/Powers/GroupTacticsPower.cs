using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

public class GroupTacticsPower : SlimeBossPowerModel
{
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        return count + Amount;
    }
}
