using Automaton.AutomatonCode.Compile;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Interfaces;

public interface ICompilable
{
    IEnumerable<Compilable> Compilations { get; }

    string CompileString(CardModel card)
    {
        return string.Join("\n", Compilations.Select(c => c.GetDescription(card).GetFormattedText()));
    }
}
