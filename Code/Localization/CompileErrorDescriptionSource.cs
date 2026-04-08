using Downfall.Code.Cards.CardModels;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Localization;

public class CompileErrorDescriptionSource : IExtraDescriptionSource
{
    public IEnumerable<string> GetLines(CardModel card)
    {
        if (card is not ICompilableError || ((AutomatonCardModel)card).SuppressCompileError) yield break;
        var loc = ICompilableError.BuildErrorLocString((AutomatonCardModel)card);
        if (loc == null) yield break;
        yield return
            $"[gold]{new LocString("static_hover_tips", "DOWNFALL-COMPILE_ERROR.title").GetFormattedText()}[/gold] - {loc.GetFormattedText()}";
    }
}