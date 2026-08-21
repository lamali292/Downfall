using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Events;

public interface IAfterCardStashed
{
    Task AfterCardsStashed( PlayerChoiceContext ctx,Player player, IEnumerable<CardModel> stashedCards, IEnumerable<CardModel> overflowCards);
}