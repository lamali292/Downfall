using Downfall.DownfallCode.Localization;
using Guardian.GuardianCode.Cards.Abstract;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Localization;

public class GemDescriptionSource : IExtraDescriptionSource
{
    private static LocString EmptyGemDescription =>
        new("gems", GuardianMainFile.ModId.ToUpperInvariant() + "-EMPTY_SLOT.description");

    public IEnumerable<string> GetLines(CardModel card)
    {
        if (card is not IGemSocketCard gc) yield break;
        for (var i = 0; i < gc.GemSlots; i++)
            if (i < gc.Gems.Count)
            {
                var text = gc.Gems[i].GetFormattedText(true);
                if (text.Equals(""))
                    text = "-";
                yield return $"❮ {text} ❯";
            }

            else
            {
                yield return EmptyGemDescription.GetFormattedText();
            }

        if (card is not IGemCard gemCard) yield break;
        if (card.IsMutable)
            yield return gemCard.GemModel.GetFormattedText();
        else
            yield return gemCard.CanonicalGemModel.GetFormattedText();
    }
}