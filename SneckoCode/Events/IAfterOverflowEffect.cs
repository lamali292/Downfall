using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Snecko.SneckoCode.Events;

public interface IAfterOverflowEffect
{
    Task AfterOverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay, CardModel card);
}