using Downfall.Code.Abstract;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Collector;

public class KarmaPower : CollectorPowerModel
{
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side || !CombatState.Enemies.Any(e => e.IsAfflicted())) return;
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Move | ValueProp.Unpowered, null);
    }
}