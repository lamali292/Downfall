using Automaton.AutomatonCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Automaton.AutomatonCode.Functions;

/// <summary>
///     One thing a Function does or says, as seen by <see cref="FunctionCard" />: it plays, it shows a line
///     of text, it shows tooltips, and it takes part in the Function's type and target. The Function knows
///     nothing about where a contribution came from; <see cref="FunctionAssembler" /> creates them.
///     A contribution refers to the Function's vars by name and is evaluated against whichever Function is
///     asked, so a cloned Function (whose vars are cloned) keeps working with the same list.
/// </summary>
public sealed class FunctionContribution
{
    /// <summary>Which keyword this contribution belongs to (Encode or Compile); the Function only groups lines by it.</summary>
    public required CardKeyword Keyword { get; init; }

    /// <summary>Ascending; the contribution list is sorted by this, which is also the play order.</summary>
    public required int Order { get; init; }

    /// <summary>Name of the Function var that must be above 0 for this contribution to apply; null = always applies.</summary>
    public string? ActiveVar { get; init; }

    /// <summary>Card type this contribution asks for. The Function takes the strongest (Power, Attack, Skill).</summary>
    public CardType? Type { get; init; }

    /// <summary>Target this contribution needs. The Function takes AnyEnemy, then AllEnemies, then Self.</summary>
    public TargetType? Target { get; init; }

    /// <summary>Playing stops after this contribution resolves.</summary>
    public bool EndsSequence { get; init; }

    /// <summary>The Function targets Self regardless of the other contributions.</summary>
    public bool ForcesSelfTarget { get; init; }

    public bool GainsBlock { get; init; }

    /// <summary>Whether the line is part of the Function's card text (otherwise it is only listed in the sequence display).</summary>
    public bool ShownOnCard { get; init; }

    public Func<FunctionCard, PlayerChoiceContext, Creature?, CardPlay?, Task>? Play { get; init; }
    public Func<FunctionCard, IEnumerable<IHoverTip>>? HoverTips { get; init; }
    public Func<FunctionCard, string?>? Line { get; init; }

    public bool IsActive(FunctionCard function)
    {
        return ActiveVar == null || function.DynamicVars[ActiveVar].BaseValue > 0;
    }
}
