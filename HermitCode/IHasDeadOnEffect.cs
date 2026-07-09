using Hermit.HermitCode.Cards;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.Patches;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode;

public interface IHasDeadOnEffect
{
    
    bool IsDeadOn => ( ((CardModel)this).Pile?.Type == PileType.Hand && IsDeadOnInHand) || (((CardModel)this).Pile?.Type == PileType.Play && WasThisPlayedDeadOn);
    
    bool IsDeadOnInHand => HermitCmd.IsDeadOnInCurrentHandState((CardModel)this);
    bool WasThisPlayedDeadOn =>
        DeadOnPatch.LastPlayed == (CardModel)this && DeadOnPatch.LastWasDeadOn;

    Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay cardPlay);
}