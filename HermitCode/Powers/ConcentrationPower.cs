using Hermit.HermitCode.Core;
using Hermit.HermitCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Powers;

/// <summary>
///     The next Dead On card played this turn triggers its effect regardless of position.
///     Wears off at end of turn.
/// </summary>
public sealed class ConcentrationPower : HermitPowerModel, IShouldTriggerDeadOn, IAfterDeadOnTrigger
{
 
    public override async Task AfterTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side) return;
        await PowerCmd.Remove(this);
    }

    public bool ShouldTriggerDeadOn(CardModel card)
        => card.Owner.Creature == Owner;

    public async Task AfterDeadOnTrigger(PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay)
    {
        await PowerCmd.ModifyAmount(ctx, this, -1, Owner, cardPlay?.Card);
    }
}