using Automaton.AutomatonCode.Cards.Token;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Compile;

/// <summary>The Function costs the source card's Energy value (Null Pointer). The last source to set a cost wins.</summary>
public class FunctionCostCompile : FunctionModifierCompile
{
    public override string Id => "FUNCTION_COST_COMPILE";
    public override int Order => -1;

    public override void ApplyCompile(FunctionCard fn, CardModel source)
    {
        fn.EnergyCost.SetCustomBaseCost(source.DynamicVars.Energy.IntValue);
    }
}
