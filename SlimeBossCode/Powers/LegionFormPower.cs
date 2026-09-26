using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Powers;

public class LegionFormPower : SlimeBossPowerModel
{
    public LegionFormPower()
    {
        WithTip(SlimeBossTip.Command);
    }

    public override async Task AfterPlayerTurnStartEarly(PlayerChoiceContext ctx, Player player)
    {
        if ( player.Creature != Owner) return;
        Flash();
        await SlimeBossCmd.CommandAll(ctx, player, Amount);
    }
}