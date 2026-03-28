using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Interfaces;

public interface IChantable
{
    bool HasChanted { get; set; }
    Task OnChant(PlayerChoiceContext ctx, CardPlay cardPlay);
}