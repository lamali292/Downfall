using Automaton.AutomatonCode.Interfaces;
using Downfall.DownfallCode.Localization;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Localization;

public class EncodeDescriptionSource : IExtraDescriptionSource
{
    public IEnumerable<string> GetLines(CardModel card)
    {
        if (card is not IEncodable { AutoEncode: true } encodable) yield break;
        var encode = encodable.EncodeLocString;
        if (encode == null) yield break;
        var title = new LocString("static_hover_tips", "AUTOMATON-ENCODE.title").GetFormattedText();
        var period = new LocString("card_keywords", "PERIOD").GetFormattedText();
        yield return $"{encode.GetFormattedText()}\n[gold]{title}[/gold]{period}";
    }
}