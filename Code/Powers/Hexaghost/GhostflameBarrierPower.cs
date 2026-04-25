using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Hexaghost;

public class GhostflameBarrierPower : HexaghostPowerModel
{
    public override async Task AfterAttack(PlayerChoiceContext ctx, AttackCommand command)
    {
        if (command.Attacker == null) return;
        var list = command.Results.Where(r => r.Receiver == Owner).ToList();
        if (list.Count == 0 ) return;
        await PowerCmd.Apply<SoulBurnPower>(ctx, command.Attacker, Amount, Owner, null);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side) return;
        await PowerCmd.Remove(this);
    }
}