using Automaton.AutomatonCode.Cards;
using Automaton.AutomatonCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Events;

public interface IOnCompile
{
    Task OnCompile(PlayerChoiceContext ctx, IReadOnlyList<CardModel> snapshot, FunctionCard functionCard,
        CardPlay cardPlay);
}