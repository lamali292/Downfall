using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Events;

public interface IOnPyre
{
    Task OnPyre(PlayerChoiceContext ctx, CardModel card, CardModel pyred);
}