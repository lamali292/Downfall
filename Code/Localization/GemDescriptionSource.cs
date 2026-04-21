using Downfall.Code.Abstract.CardModels;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Localization;

public class GemDescriptionSource : IExtraDescriptionSource
{
    public IEnumerable<string> GetLines(CardModel card)
    {
        if (card is not GuardianCardModel guardianCardMode) yield break;
        foreach (var description in guardianCardMode.Gems.Select(gemModel => gemModel.Description))
        {
            card.DynamicVars.AddTo(description);
            var prefix = EnergyIconHelper.GetPrefix(card);
            description.Add("energyPrefix", prefix);
            description.Add("singleStarIcon", "[img]res://images/packed/sprite_fonts/star_icon.png[/img]");
            yield return description.GetFormattedText();
        }
    }
}