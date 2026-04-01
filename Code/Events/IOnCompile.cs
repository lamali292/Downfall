using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Events;

public interface IOnCompile
{
    Task OnCompile(PlayerChoiceContext ctx, IReadOnlyList<AutomatonCardModel> snapshot, FunctionCard functionCard,
        CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }
}