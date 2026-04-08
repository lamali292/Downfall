using Downfall.Code.Core;
using Downfall.Code.Core.Champ;
using Downfall.Code.Extensions;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Localization;

public class FinisherDescriptionSource : IExtraDescriptionSource
{
    private const string DownfallTable = "downfall";

    public IEnumerable<string> GetLines(CardModel card)
    {
        if (!card.Tags.Contains(DownfallTag.Finisher)) yield break;
        var stance = card.IsCanonical || card.Owner == null
            ? DownfallModelDb.ChampStance<NoChampStance>()
            : card.Owner.ChampStance();
        yield return new LocString(DownfallTable, $"{stance.Id.Entry}.finisher").GetFormattedText();
    }
}