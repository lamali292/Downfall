using Automaton.AutomatonCode.Core;
using Downfall.DownfallCode.Compatibility;
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

    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (creator == null || creator.Creature != Applier)
            return;
        Flash();
        await CompatibilityCreatureCmd.Damage(new BlockingPlayerChoiceContext(), Owner, Amount,
            DamageProps.nonCardHpLoss, card.Owner.Creature, card, null);
        await PowerCmd.Remove(this);
    }
}