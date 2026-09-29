using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Powers;

public class InevitableDemisePower() : CollectorPowerModel(PowerType.Debuff), IModifyCollectorMiasmaIncrement
{
    public int ModifyCollectorMiasmaIncrement(Creature creature, int current)
    {
        return creature == Owner ? current + Amount : current;
    }

    // ModifyCollectorMiasmaIncrement only ever gets consulted from inside MiasmaPower's own
    // end-of-turn trigger - with no Miasma on the owner yet, that trigger never runs, so the
    // increase-Miasma-by-X debuff would otherwise do nothing until something else applies Miasma
    // first. Grant a single stack directly instead, so it's never a dead debuff.
    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;
        if (Owner.HasPower<MiasmaPower>()) return;
        await PowerCmd.Apply<MiasmaPower>(choiceContext, Owner, Amount, Owner, null);
    }
}