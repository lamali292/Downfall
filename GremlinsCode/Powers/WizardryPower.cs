using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Gremlins.GremlinsCode.Powers;

public class WizardryPower : GremlinsPowerModel
{
    public override async Task AfterSideTurnStart(CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side) return;
        var ctx = new BlockingPlayerChoiceContext();
        Flash();
        await PowerCmd.Apply<WizPower>(ctx, Owner, Amount, Owner, null);
    }
}