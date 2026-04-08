using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Events;

public interface IOnFinisher
{
    Task OnFinisher(PlayerChoiceContext ctx, CardPlay cardPlay);
}