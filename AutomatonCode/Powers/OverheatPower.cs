using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Powers;

public class OverheatPower : AutomatonPowerModel
{
    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    protected override async Task AfterCardGeneratedForCombat(PlayerChoiceContext ctx, CardModel card, Player? creator)
    {
        if (creator == null || creator.Creature != Applier)
            return;
        Flash();
        await CreatureCmd.Damage(ctx, Owner, Amount,
            ValueProp.Unblockable | ValueProp.Unpowered, card.Owner.Creature, card);
    }
}