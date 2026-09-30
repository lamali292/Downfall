using Automaton.AutomatonCode.Cards.Token;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Encode;

/// <summary>The Function replays once more when the source card is the last one (Terminator).</summary>
public class ReplayAtEndEncode : Encodable
{
    public override string Id => "REPLAY_AT_END_ENCODE";

    public override void ApplyEncode(FunctionCard function, CardModel sourceCard, FunctionPosition position)
    {
        if (position == FunctionPosition.End) function.BaseReplayCount += 1;
    }
}
