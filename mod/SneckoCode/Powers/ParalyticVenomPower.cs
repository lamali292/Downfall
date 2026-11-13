using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Powers;

public class ParalyticVenomPower : SneckoPowerModel
{

    public ParalyticVenomPower()
    {
        WithTip<VenomPower>();
    }
    
    public override async Task AfterAttack(PlayerChoiceContext ctx, AttackCommand command)
    {
        if (command.Attacker != Owner || !command.DamageProps.IsPoweredAttack()) return;
        foreach (var damageResult in command.Results.SelectMany( e => e).Where( e => e.UnblockedDamage > 0))
        {
            await PowerCmd.Apply<VenomPower>(ctx, damageResult.Receiver, Amount, Owner, null);
        }
        Flash();
    }
}