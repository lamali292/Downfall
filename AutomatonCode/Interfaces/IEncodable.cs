using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Interfaces;

public interface IEncodable
{
    void ApplyEncode(FunctionCard function, FunctionPosition position)
    {
        
    }

    IEnumerable<Encodable> Encodings { get; }

    bool CanPlayerEncode => true;

    string EncodeString(CardModel card) => string.Join("\n", Encodings.Select(e => e.GetDescription(card).GetFormattedText())); 

}