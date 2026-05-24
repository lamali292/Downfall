using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Powers;

public class CleanCodePower : AutomatonPowerModel
{
    public CleanCodePower()
    {
        WithTip(AutomatonTip.Stash);
    }

    public override async Task BeforeFlush(PlayerChoiceContext ctx, Player player)
    {
        if (Owner != player.Creature) return;
        Flash();
        await StashCmd.StashUpTo(ctx, player, Amount, this);
    }
}