using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gremlins.GremlinsCode.Powers;

public class BashedPower : GremlinsPowerModel
{
    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    protected override async Task AfterBlockGained(PlayerChoiceContext ctx, Creature creature, decimal amount,
        ValueProp props, CardModel? cardSource)
    {
        if (creature != Applier) return;
        await CreatureCmd.Damage(ctx, Owner, Amount, DamageProps.nonCardHpLoss, creature);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side) return;
        await PowerCmd.Remove(this);
    }
}