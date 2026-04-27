using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Interfaces;

public interface IChantable
{
    bool HasChanted { get; set; }
    Task PlayChantEffect(PlayerChoiceContext ctx, CardPlay cardPlay);
}