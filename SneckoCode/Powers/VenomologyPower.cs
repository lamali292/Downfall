using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Powers;

/// <summary>Concoct, but applying Venom instead of Poison.</summary>
public class VenomologyPower : SneckoPowerModel
{
    public VenomologyPower()
    {
        WithTip<VenomPower>();
    }

    public override async Task AfterDamageGiven(PlayerChoiceContext ctx, Creature? dealer, DamageResult result,
        ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack() || result.UnblockedDamage <= 0) return;
        await PowerCmd.Apply<VenomPower>(ctx, target, Amount, Owner, null);
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (Owner.Side != side)
            await PowerCmd.Remove(this);
    }
}
