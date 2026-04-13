using Downfall.Code.Core;
using Downfall.Code.Core.Champ;
using Downfall.Code.Extensions;
using Downfall.Code.Keywords;
using Downfall.Code.Powers.Champ;
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

        var locString = new LocString(DownfallTable, $"{stance.Id.Entry}.finisher");

        var berserkerBonus = card.Owner?.Creature.GetPower<ArenaMasteryBerserkerPower>()?.Amount ?? 0;
        var defensiveBonus = card.Owner?.Creature.GetPower<ArenaMasteryDefensivePower>()?.Amount ?? 0;
        locString.Add("strength", 1 + berserkerBonus);
        locString.Add("block", 6 + defensiveBonus);
        
        yield return locString.GetFormattedText();
    }
}