using Automaton.AutomatonCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Encode;

/// <summary>The Function gains Retain. No value, no play step: it only edits the Function when it is assembled.</summary>
public class RetainEncode : Encodable
{
    public override string Id => "RETAIN_ENCODE";

    public override void ApplyEncode(FunctionCard function, CardModel sourceCard, FunctionPosition position)
    {
        function.AddKeyword(CardKeyword.Retain);
    }
}
