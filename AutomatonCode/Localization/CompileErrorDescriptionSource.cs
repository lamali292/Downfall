using Automaton.AutomatonCode.Cards;
using Automaton.AutomatonCode.Interfaces;
using Downfall.DownfallCode.Localization;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Localization;

public class CompileErrorDescriptionSource : IExtraDescriptionSource
{
    public IEnumerable<string> GetLines(CardModel card)
    {
        if (card is not ICompilableError || ((AutomatonCardModel)card).SuppressCompileError) yield break;
        var loc = ICompilableError.BuildErrorLocString((AutomatonCardModel)card);
        if (loc == null) yield break;
        var compileErrorTitle = new LocString("static_hover_tips", "AUTOMATON-COMPILE_ERROR.title").GetFormattedText();
        var colon = new LocString("card_keywords", "COLON").GetFormattedText();
        yield return
            $"[gold]{compileErrorTitle}[/gold]{colon}{loc.GetFormattedText()}";
    }
}