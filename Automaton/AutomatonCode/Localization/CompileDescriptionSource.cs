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
        yield return
            $"[gold]{new LocString("static_hover_tips", "DOWNFALL-COMPILE.title").GetFormattedText()}[/gold] - {loc.GetFormattedText()}";
    }
}