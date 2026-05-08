using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Powers;

public class GhostflameBarrierPower : HexaghostPowerModel
{
    public override async Task AfterAttack(PlayerChoiceContext ctx, AttackCommand command)
    {
        if (command.Attacker == null) return;
        var list = command.Results.SelectMany(r => r).Where(r => r.Receiver == Owner).ToList();
        if (list.Count == 0) return;
        await PowerCmd.Apply<SoulBurnPower>(ctx, command.Attacker, Amount, Owner, null);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side) return;
        await PowerCmd.Remove(this);
    }
}