using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Powers;

public class CleanCodePower : AutomatonPowerModel
{
    public CleanCodePower()
    {
        WithTip(AutomatonTip.Stash);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != Owner.Side || Owner.Player == null) return;
        Flash();
        await AutomatonCmd.StashUpTo(ctx, Owner.Player, Amount, this);
    }
}