using Downfall.DownfallCode.Localization;
using Guardian.GuardianCode.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Localization;

public class GemDescriptionSource : IExtraDescriptionSource
{
    private static LocString EmptyGemDescription =>
        new("gems", GuardianMainFile.ModId.ToUpperInvariant() + "-EMPTY_SLOT.description");

    public IEnumerable<string> GetLines(CardModel card)
    {
        if (card is not GuardianCardModel gc) yield break;
        for (var i = 0; i < gc.GemSlots; i++)
            if (i < gc.Gems.Count)
                yield return gc.Gems[i].GetFormattedText();
            else
                yield return EmptyGemDescription.GetFormattedText();
    }
}