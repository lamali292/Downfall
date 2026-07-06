using Automaton.AutomatonCode.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Encode;

public class ConstructorEncode() : SimpleBlockEncode(5, 2), IOnEncode
{
    public async Task OnCardEncoded(PlayerChoiceContext ctx, CardModel encodedCard)
    {
        if (encodedCard != Owner) return;
        DynamicVars.Block.BaseValue *= 2;
    }
}