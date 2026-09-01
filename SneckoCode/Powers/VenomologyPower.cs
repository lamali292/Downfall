using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Powers;

public class VenomologyPower : SneckoPowerModel
{
    public VenomologyPower() : base(PowerType.Debuff)
    {
        WithTip<VenomPower>();
    }

    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext ctx, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power.Owner != Owner || applier == Applier || applier?.Player == null || power.GetTypeForAmount(amount) != PowerType.Debuff) return;
        await Cmd.Wait(0.1f);
        Flash();
        await PowerCmd.Apply<VenomPower>(ctx, Owner, Amount, Owner, null);
      
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (Applier != null && !participants.Contains(Applier))
            return;
        await PowerCmd.Remove(this);
    }
}