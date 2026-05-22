using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Events;

public interface IAfterCompilingFunction
{
    Task AfterCompilingFunction(PlayerChoiceContext ctx, Player player, CardPileAddResult result, CardPlay cardPlay);
}