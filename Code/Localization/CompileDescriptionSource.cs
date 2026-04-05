using Downfall.Code.Cards.CardModels;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Localization;

public class CompileDescriptionSource : IExtraDescriptionSource
{
    public void AddDescriptionLines(CardModel card, List<string> source)
    {
        if (card is not ICompilable) return;
        var loc = ICompilable.BuildCompileLocString((AutomatonCardModel)card);
        if (loc == null) return;
        source.Add(
            $"[gold]{new LocString("static_hover_tips", "DOWNFALL-COMPILE.title").GetFormattedText()}[/gold] - {loc.GetFormattedText()}");
    }
}