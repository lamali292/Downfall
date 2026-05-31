using Hermit.HermitCode.Core;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     If you end your turn Concentrated, gain an additional energy next turn.
/// </summary>
public sealed class Spyglass : HermitRelicModel
{
    public Spyglass() : base(RelicRarity.Uncommon)
    {
        WithEnergy(1);
        WithTip<ConcentrationPower>();
    }


    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount,
        Creature? applier, CardModel? cardSource)
    {
        if (power is not ConcentrationPower || power.Owner != Owner.Creature) return Task.CompletedTask;
        Status = amount + Owner.Creature.GetPowerAmount<ConcentrationPower>() == 0
            ? RelicStatus.Normal
            : RelicStatus.Active;
        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == Owner.Creature.Side && Owner.Creature.HasPower<ConcentrationPower>())
        {
            Flash();
            await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner.Creature, DynamicVars.Energy.BaseValue,
                Owner.Creature, null);
        }
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
}