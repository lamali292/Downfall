using Hermit.HermitCode.Core;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Powers;

public sealed class ConcentrationPower : HermitPowerModel, IShouldTriggerDeadOn, IAfterDeadOnTrigger
{
    public ConcentrationPower()
    {
        WithTip(HermitKeywords.DeadOn);
    }

    public async Task AfterDeadOnTrigger(PlayerChoiceContext ctx, CardModel card)
    {
        if (card.Owner.Creature != Owner) return;
        await PowerCmd.Decrement(this);
    }

    public bool ShouldTriggerDeadOn(CardModel card)
    {
        return card.Owner.Creature == Owner;
    }


    public override async Task AfterSideTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner)) return;
        await PowerCmd.Remove(this);
    }
}