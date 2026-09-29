using Automaton.AutomatonCode.Cards.Token;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Encode;

/// <summary>The Function costs the source card's Energy value (Null Pointer). The last source to set a cost wins.</summary>
public class FunctionCostEncode : Encodable
{
    public override string Id => "FUNCTION_COST_ENCODE";

    public override void ApplyEncode(FunctionCard function, CardModel sourceCard, FunctionPosition position)
    {
        function.EnergyCost.SetCustomBaseCost(sourceCard.DynamicVars.Energy.IntValue);
    }
}
