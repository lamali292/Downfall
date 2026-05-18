using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode;

public interface IHasDeadOnEffect
{
    Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay cardPlay);
}