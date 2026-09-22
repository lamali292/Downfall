using Automaton.AutomatonCode.Interfaces;
using Downfall.DownfallCode.Localization;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Localization;

public class CompileDescriptionSource : IExtraDescriptionSource
{
    public IEnumerable<string> GetLines(CardModel card)
    {
        if (card is not ICompilable compilable) yield break;
        var text = compilable.CompileString(card);
        var title = new LocString("static_hover_tips", "AUTOMATON-COMPILE.title").GetFormattedText();
        var suffix = $"[gold]{title}[/gold]";
        var compile = new LocString("encode", "AUTOMATON-COMPILE.format");
        compile.Add("compile", suffix);
        compile.Add("text", text);
        yield return compile.GetFormattedText();
    }
}
