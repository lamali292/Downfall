using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Powers;

public class SneckoConstrictPower() : SneckoPowerModel(PowerType.Debuff)
{
    
    public override async Task AfterTurnEnd(PlayerChoiceContext ctx, CombatSide side)
    {
        if (side != Owner.Side)
            return;
        await CreatureCmd.Damage(ctx, Owner, Amount, ValueProp.Unpowered, Owner, null);
    }
}