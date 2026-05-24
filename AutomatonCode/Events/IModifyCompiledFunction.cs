using Automaton.AutomatonCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Automaton.AutomatonCode.Events;

public interface IModifyCompiledFunction
{
    bool ModifyCompiledFunction(FunctionCard function, Player player);
    Task AfterModifyCompiledFunction(FunctionCard result, Player player);
}