using Automaton.AutomatonCode.Interfaces;
using Downfall.DownfallCode.Localization;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Localization;

public class EncodeDescriptionSource : IExtraDescriptionSource
{
    public IEnumerable<string> GetLines(CardModel card)
    {
        if (card is not IEncodable encodable) yield break;
        var encode = encodable.EncodeLocString;
        var title = new LocString("static_hover_tips", "AUTOMATON-ENCODE.title").GetFormattedText();
        var period = new LocString("card_keywords", "PERIOD").GetFormattedText();
        var text = encode.GetFormattedText();
        var suffix = $"[gold]{title}[/gold]{period}";
        yield return string.IsNullOrEmpty(text) ? suffix : $"{text}\n{suffix}";
    }
}