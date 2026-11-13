using Automaton.AutomatonCode.Cards.Token;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Functions;

/// <summary>Builds a Function's title from its source cards (loc keys live in <c>encode.json</c>).</summary>
public static class FunctionNaming
{
    public static string GetTitle(IReadOnlyList<CardModel> sourceCards)
    {
        if (sourceCards is [Constructor, Separator, Terminator] or [Constructor, Separator, Separator, Terminator])
            return new LocString("encode", "AUTOMATON-PERFECTION.functionName").GetFormattedText();

        var prefix = Fragment(sourceCards, 0, ".functionPrefix", card => card.Title.ToLowerInvariant());
        var name = Fragment(sourceCards, 1, ".functionName", card => card.Title);
        var end3 = Fragment(sourceCards, 2, ".functionEnd", card => card.Title[0].ToString());
        var end4 = Fragment(sourceCards, 3, ".functionEnd", card => card.Title[0].ToString());
        var parenthesesLoc = new LocString("encode", "AUTOMATON-FUNCTION.functionParentheses");
        var parentheses = parenthesesLoc.Exists() ? parenthesesLoc.GetFormattedText() : "()";

        var functionName = new LocString("encode", "AUTOMATON-FUNCTION.title");
        functionName.Add("prefix", prefix);
        functionName.Add("name", name);
        functionName.Add("end3", end3);
        functionName.Add("end4", end4);
        functionName.Add("parentheses", parentheses);
        return functionName.GetFormattedText();
    }

    private static string Fragment(IReadOnlyList<CardModel> sourceCards, int index, string suffix,
        Func<CardModel, string> fallback)
    {
        if (sourceCards.Count <= index)
            return "";

        var loc = new LocString("encode", sourceCards[index].Id.Entry + suffix);
        return loc.Exists() ? loc.GetFormattedText() : fallback(sourceCards[index]);
    }
}
