using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Events;

public interface IOnChant
{
    Task OnCardChanted(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay);
}