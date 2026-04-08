using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Localization;

public class EncodeDescriptionSource : IExtraDescriptionSource
{
    public IEnumerable<string> GetLines(CardModel card)
    {
        if (card is not IEncodable { AutoEncode: true } encodable) yield break;
        var encode = encodable.EncodeLocString;
        if (encode == null) yield break;
        var title = new LocString("static_hover_tips", "DOWNFALL-ENCODE.title").GetFormattedText();
        yield return $"{encode.GetFormattedText()}\n[gold]{title}[/gold].";
    }
}