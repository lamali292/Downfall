using Downfall.DownfallCode.Localization;
using Guardian.GuardianCode.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Localization;

public class GemDescriptionSource : IExtraDescriptionSource
{
    private static LocString EmptyGemDescription => new("gems", GuardianMainFile.ModId.ToUpperInvariant()+"-EMPTY_SLOT.description");

    public IEnumerable<string> GetLines(CardModel card)
    {
        if (card is not GuardianCardModel gc) yield break;
        for (var i = 0; i < gc.GemSlots; i++)
        {
            var description = i < gc.Gems.Count
                ? gc.Gems[i].Description
                : EmptyGemDescription;

            card.DynamicVars.AddTo(description);
            var prefix = EnergyIconHelper.GetPrefix(card);
            description.Add("energyPrefix", prefix);
            description.Add("singleStarIcon", "[img]res://images/packed/sprite_fonts/star_icon.png[/img]");
            yield return description.GetFormattedText();
        }
    }
}