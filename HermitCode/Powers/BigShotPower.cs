using Hermit.HermitCode.Cards;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Powers;

/// <summary>
///     At the start of your turn, you can Exhaust a card to gain 8 Block.
///     Stacks increase block gained (8 per stack).
/// </summary>
public sealed class BigShotPower : HermitPowerModel, IAfterDeadOnTrigger
{
    public async Task AfterDeadOnTrigger(PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay)
    {
        await PowerCmd.Apply<VigorPower>(ctx, Owner, Amount, Owner, cardPlay?.Card);
    }
}