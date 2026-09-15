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
        var period = new LocString("card_keywords", "PERIOD").GetFormattedText();
        var suffix = $"[gold]{title}[/gold]";
        yield return string.IsNullOrEmpty(text) ? suffix : $"{suffix} - {text}";
    }
}
