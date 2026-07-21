using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Encode;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Interfaces;

public interface IEncodable
{
    IEnumerable<Encodable> Encodings { get; }

    bool CanPlayerEncode => true;

    void ApplyEncode(FunctionCard function, FunctionPosition position)
    {
    }

    string EncodeString(CardModel card)
    {
        return string.Join("\n", Encodings.Select(e => e.GetDescription(card).GetFormattedText()));
    }
}