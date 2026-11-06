using Automaton.AutomatonCode.Cards.Token;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Encode;

/// <summary>
///     Adds one of the source card's own vars to a Function var, but only when the source card sits at a
///     given slot of the sequence (first, middle or last).
/// </summary>
public abstract class PositionBonusEncode : Encodable
{
    protected abstract FunctionPosition Position { get; }

    /// <summary>Name of the var on the source card that holds the bonus.</summary>
    protected abstract string SourceVarName { get; }

    /// <summary>The Function var that receives the bonus.</summary>
    protected abstract DynamicVar Target(FunctionCard function);

    public override void ApplyEncode(FunctionCard function, CardModel sourceCard, FunctionPosition position)
    {
        if (position == Position)
            Target(function).BaseValue += sourceCard.DynamicVars[SourceVarName].BaseValue;
    }
}

/// <summary>Constructor: extra Block when it is the first card.</summary>
public class StartBlockEncode : PositionBonusEncode
{
    public override string Id => "START_BLOCK_ENCODE";
    protected override FunctionPosition Position => FunctionPosition.Start;
    protected override string SourceVarName => "ExtraBlock";

    protected override DynamicVar Target(FunctionCard function)
    {
        return function.DynamicVars.Block;
    }
}

/// <summary>Separator: extra Damage when it is in the middle.</summary>
public class MiddleDamageEncode : PositionBonusEncode
{
    public override string Id => "MIDDLE_DAMAGE_ENCODE";
    protected override FunctionPosition Position => FunctionPosition.Middle;
    protected override string SourceVarName => "ExtraDamage";

    protected override DynamicVar Target(FunctionCard function)
    {
        return function.DynamicVars.Damage;
    }
}
