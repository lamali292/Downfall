using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Events;

public interface IAfterCardPyred
{
    Task AfterCardPyred(PlayerChoiceContext ctx, CardModel card, CardModel pyred);
}