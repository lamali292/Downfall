using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.DownfallCode.Powers;

public class NextTurnStunnedPower() : DownfallPowerModel(PowerType.Debuff)
{
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side) {
            await PowerCmd.Apply<StunnedPower>(ctx, Owner, 1, Owner, null);
            await PowerCmd.Decrement(this);
        }
    }
}
