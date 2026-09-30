using Automaton.AutomatonCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Compile;

/// <summary>The Function gains Retain (Frontload).</summary>
public class RetainCompile : FunctionModifierCompile
{
    public override string Id => "RETAIN_COMPILE";
    public override int Order => -2;

    public override void ApplyCompile(FunctionCard fn, CardModel source)
    {
        fn.AddKeyword(CardKeyword.Retain);
    }
}
