using Hermit.HermitCode.Cards;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode;

public interface IHasDeadOnEffect
{
    bool IsDeadOn => HermitCmd.IsDeadOnInCurrentHandState((CardModel)this) ||
                     (PatchDeadOnCapture.LastPlayed == (CardModel)this && PatchDeadOnCapture.LastWasDeadOn);

    Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay cardPlay);
}