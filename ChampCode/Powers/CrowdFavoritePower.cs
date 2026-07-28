using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Powers;

public class CrowdFavoritePower : ChampPowerModel
{
    public CrowdFavoritePower()
    {
        WithTip<VigorPower>();
    }
    
    public override Task AfterAttack(PlayerChoiceContext ctx, AttackCommand command)
    {
        if (command.Attacker is not { IsPlayer: true } || command.Attacker == Owner)
            return Task.CompletedTask;
        Flash();
        return PowerCmd.Apply<VigorPower>(ctx, Owner, Amount, Owner, null);
    }
}