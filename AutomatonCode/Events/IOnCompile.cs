using Automaton.AutomatonCode.Cards;
using Automaton.AutomatonCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Events;

public interface IOnCompile
{
    Task OnCompile(PlayerChoiceContext ctx, IReadOnlyList<AutomatonCardModel> snapshot, FunctionCard functionCard,
        CardPlay cardPlay);
}