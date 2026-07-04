using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Hermit.HermitCode.Powers;

public sealed class DrawFewerCardsNextTurnPower() : HermitPowerModel(PowerType.Debuff)
{
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != Owner.Player || AmountOnTurnStart == 0) return count;
        return Math.Max(0m, count - 1);
    }

    public override async Task AfterSideTurnStart(CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner) || AmountOnTurnStart == 0) return;
        await PowerCmd.Decrement(this);
    }
}