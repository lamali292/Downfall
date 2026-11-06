using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Automaton.AutomatonCode.CustomEnums;

/// <summary>
///     Encode: the card can be put into a Function, and is encoded when played.
///     Compile: the card has an effect that fires once when the Function is created.
///     Both texts are added to the card by the Encode/Compile description sources, so no automatic keyword line.
///     Localization is provided in card_keywords.json.
/// </summary>
public static class AutomatonKeyword
{
    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Encode;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Compile;
}
