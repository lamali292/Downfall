using Hermit.HermitCode.Core;
using Hermit.HermitCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Powers;

public sealed class SmokingBarrelPower : HermitPowerModel, IAfterDeadOnTrigger
{
    public async Task AfterDeadOnTrigger(PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay)
    {
        if (card.Owner.Creature != Owner) return;
        Flash();
        await PowerCmd.Apply<VigorPower>(ctx, Owner, Amount, Owner, cardPlay.Card);
    }
}