using Automaton.AutomatonCode.Cards;
using Automaton.AutomatonCode.Interfaces;
using Downfall.DownfallCode.Localization;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Localization;

public class CompileDescriptionSource : IExtraDescriptionSource
{
    public IEnumerable<string> GetLines(CardModel card)
    {
        if (card is not ICompilable) yield break;
        var loc = ICompilable.BuildCompileLocString((AutomatonCardModel)card);
        if (loc == null) yield break;
        var compileTitle = new LocString("static_hover_tips", "AUTOMATON-COMPILE.title").GetFormattedText();
        var colon = new LocString("card_keywords", "COLON").GetFormattedText();
        yield return
            $"[gold]{compileTitle}[/gold]{colon}{loc.GetFormattedText()}";
    }
}