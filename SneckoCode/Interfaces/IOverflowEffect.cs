using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Snecko.SneckoCode.Interfaces;

public interface IHasOverflowEffect
{
    Task OverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay);
}