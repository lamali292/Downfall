using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Localization;

public class EncodeDescriptionSource : IExtraDescriptionSource
{
    public void AddDescriptionLines(CardModel card, List<string> source)
    {
        if (card is not IEncodable { AutoEncode: true } encodable) return;
        var encode = encodable.EncodeLocString;
        if (encode == null) return;
        var title = new LocString("static_hover_tips", "DOWNFALL-ENCODE.title").GetFormattedText();
        var insertIndex = source.FindIndex(l => !l.StartsWith("[gold]") || !l.EndsWith("."));
        source.Insert(insertIndex < 0 ? 0 : insertIndex, $"{encode.GetFormattedText()}\n[gold]{title}[/gold].");
    }
}