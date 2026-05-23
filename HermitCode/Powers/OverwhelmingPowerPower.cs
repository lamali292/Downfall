using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Powers;

public sealed class OverwhelmingPowerPower() : HermitPowerModel(PowerType.Debuff)
{
    public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player) return;
        var player = Owner.Player;
        if (player?.PlayerCombatState?.Energy != 0) return;
        Flash();
        await CreatureCmd.Damage(ctx, Owner, Amount,
            ValueProp.Unblockable | ValueProp.Unpowered, Owner, null);
    }
}