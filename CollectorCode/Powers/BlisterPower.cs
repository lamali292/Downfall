using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Collector.CollectorCode.Powers;

/// <summary>
///     Once per turn, at the end of its owner's turn, the owner loses HP equal to the Blister amount (ignoring
///     Block), then Blister drops by 1. The amount is the damage directly.
/// </summary>
public class BlisterPower() : CollectorPowerModel(PowerType.Debuff)
{
    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;
        await Trigger();
    }

    /// <summary>Deals the Blister damage and decays it by 1. Called once per turn.</summary>
    public async Task Trigger()
    {
        if (Amount <= 0) return;

        await CompatibilityCreatureCmd.Damage(new BlockingPlayerChoiceContext(), Owner, Amount,
            DamageProps.nonCardHpLoss, null, null, null);
        Flash();
        if (Owner.IsAlive) await PowerCmd.Decrement(this);
    }
}
