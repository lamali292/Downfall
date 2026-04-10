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

        if (card.IsMutable)
        {
            var creature = card.Owner?.Creature;
            var berserkerBonus = creature?.GetPower<ArenaMasteryBerserkerPower>()?.Amount ?? 0;
            var defensiveBonus = creature?.GetPower<ArenaMasteryDefensivePower>()?.Amount ?? 0;
            locString.Add("strength", BerserkerChampStance.BaseFinisherAmount + berserkerBonus);
            locString.Add("block", DefensiveChampStance.BaseFinisherAmount + defensiveBonus);
        }
        else
        {
            locString.Add("strength", BerserkerChampStance.BaseFinisherAmount);
            locString.Add("block", DefensiveChampStance.BaseFinisherAmount);
        }
        
        yield return locString.GetFormattedText();
    }
}