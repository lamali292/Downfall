using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Interfaces;

public interface ICompilable
{
    Task OnCompile(PlayerChoiceContext context);
}