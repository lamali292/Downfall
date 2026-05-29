using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Events;

public interface IOnEncode
{
    Task OnCardEncoded(PlayerChoiceContext ctx, CardModel encodedCard);
}