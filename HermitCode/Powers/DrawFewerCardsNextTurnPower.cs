using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Powers;

public sealed class DrawFewerCardsNextTurnPower() : HermitPowerModel(PowerType.Debuff)
{
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != Owner.Player || AmountOnTurnStart == 0) return count;
        return Math.Max(0m, count - Amount);
    }

    protected override async Task AfterSideTurnStart(PlayerChoiceContext ctx, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side || AmountOnTurnStart == 0) return;
        await PowerCmd.Remove(this);
    }
}