using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Powers;

public class ProtectTheBossPower : SlimeBossPowerModel
{
    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (Owner.Player == null || !participants.Contains(Owner)) return;
        var count = Owner.Player.SlimeCount * Amount;
        if (count <= 0) return;
        await CreatureCmd.GainBlock(Owner, count, BlockProps.nonCardUnpowered, null);
    }
}
