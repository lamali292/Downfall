using Automaton.AutomatonCode.Cards.Status;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Powers;

public class MaxOutputPower : AutomatonPowerModel
{
    public MaxOutputPower()
    {
        WithTip(AutomatonTip.Stash);
        WithTip(typeof(Error));
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (Owner.Player != player || Owner.Player == null) return;
        Flash();
        await StashCmd.Stash<Error>(player, Amount);
    }
}