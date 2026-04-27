using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Powers;
using Downfall.DownfallCode.Localization;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Localization;

public class FinisherDescriptionSource : IExtraDescriptionSource
{
    private const string DownfallTable = "downfall";

    public IEnumerable<string> GetLines(CardModel card)
    {
        if (!card.Tags.Contains(ChampTag.Finisher)) yield break;
        var stance = card.IsCanonical || card.Owner == null
            ? ChampModelDb.ChampStance<ChampNoStance>()
            : card.Owner.ChampStance();

        var locString = new LocString(DownfallTable, $"{stance.Id.Entry}.finisher");

        if (card.IsMutable)
        {
            var creature = card.Owner?.Creature;
            var berserkerBonus = creature?.GetPower<ArenaMasteryBerserkerPower>()?.Amount ?? 0;
            var defensiveBonus = creature?.GetPower<ArenaMasteryDefensivePower>()?.Amount ?? 0;
            locString.Add("strength", ChampBerserkerStance.BaseFinisherAmount + berserkerBonus);
            locString.Add("block", ChampDefensiveStance.BaseFinisherAmount + defensiveBonus);
        }
        else
        {
            locString.Add("strength", ChampBerserkerStance.BaseFinisherAmount);
            locString.Add("block", ChampDefensiveStance.BaseFinisherAmount);
        }

        yield return locString.GetFormattedText();
    }
}