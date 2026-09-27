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
    
    public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;
        var player = Owner.Player;
        if (player == null) return;
        Flash();
        await StashCmd.StashUpTo(ctx, player, Amount, this);
    }
}