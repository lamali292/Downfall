using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Encode;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Interfaces;

public interface IEncodable
{
    IEnumerable<Encodable> Encodings { get; }

    bool CanPlayerEncode => true;

    void ApplyEncode(FunctionCard function, FunctionPosition position)
    {
    }

    /// <summary>
    /// Description of the change <see cref="ApplyEncode"/> makes to the Function on compile (Retain, cost, ...).
    /// Loc key <c>encode: &lt;ID&gt;.compile</c>; null when the card has no such entry.
    /// </summary>
    LocString? CompileDescription(CardModel card)
    {
        var loc = new LocString("encode", card.Id.Entry + ".compile");
        if (!loc.Exists()) return null;
        card.DynamicVars.AddTo(loc);
        return loc;
    }

    string EncodeString(CardModel card)
    {
        return string.Join("\n", Encodings.Select(e => e.GetDescription(card).GetFormattedText()));
    }
}