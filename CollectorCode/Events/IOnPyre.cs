using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Events;

public interface IOnPyre
{
    Task OnPyre(PlayerChoiceContext ctx, CardModel card, CardModel pyred);
}