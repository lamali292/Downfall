using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Interfaces;
using Downfall.DownfallCode.Localization;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Localization;

/// <summary>
///     The card's Compile line: its Compile effects (only with the Compile keyword) followed by what its Encode
///     effects change about the Function (Retain, a fixed cost, ...). The latter are the same notes the Function
///     lists, so a card's loc file does not repeat them.
/// </summary>
public class CompileDescriptionSource : IExtraDescriptionSource
{
    public IEnumerable<string> GetLines(CardModel card)
    {
        var texts = new List<string>();
        if (card.Keywords.Contains(AutomatonKeyword.Compile) && card is ICompilable compilable)
            texts.Add(compilable.CompileString(card));
        if (card is IEncodable encodable)
            texts.AddRange(encodable.Encodings
                .Select(e => e.GetFunctionNote(card)?.GetFormattedText())
                .OfType<string>());
        texts.RemoveAll(string.IsNullOrEmpty);
        if (texts.Count == 0) yield break;

        var title = new LocString("card_keywords", "AUTOMATON-COMPILE.title").GetFormattedText();
        var suffix = $"[gold]{title}[/gold]";
        var compile = new LocString("encode", "AUTOMATON-COMPILE.format");
        compile.Add("compile", suffix);
        compile.Add("text", string.Join("\n", texts));
        yield return compile.GetFormattedText();
    }
}
