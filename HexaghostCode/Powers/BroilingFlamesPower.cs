using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Powers;

public class BroilingFlamesPower : HexaghostPowerModel
{
    public override async Task AfterAttack(PlayerChoiceContext ctx, AttackCommand command)
    {
        if (command.Results.SelectMany(r => r).All(e => e.Receiver != Owner)) return;
        await PowerCmd.Apply<SoulBurnPower>(ctx, Owner, Amount, command.Attacker, null);
    }
}