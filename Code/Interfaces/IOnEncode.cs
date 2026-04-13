using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Interfaces;

public interface IOnEncode
{
    Task OnCardEncoded(PlayerChoiceContext ctx, CardModel encodedCard, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }
}