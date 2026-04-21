using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Collector;

public class ScorchedPower : CollectorPowerModel
{
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side) return;
        await PowerCmd.Apply<CollectorDoomPower>(Owner, Amount, Applier, null);
    }
}