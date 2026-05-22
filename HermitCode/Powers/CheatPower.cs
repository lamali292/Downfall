using Hermit.HermitCode.Core;
using Hermit.HermitCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Powers;

public sealed class CheatPower() : HermitPowerModel(PowerType.Buff, PowerStackType.Single), IShouldTriggerDeadOn
{
    public bool ShouldTriggerDeadOn(CardModel card)
    {
        return card.Owner.Creature == Owner;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return PowerCmd.Remove(this);
    }
}