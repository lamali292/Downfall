using BaseLib.Extensions;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Stance;
using Downfall.DownfallCode.Localization;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Localization;

public class FinisherDescriptionSource : IExtraDescriptionSource
{
    public IEnumerable<string> GetLines(CardModel card)
    {
        if (!card.Tags.Contains(ChampTag.Finisher)) yield break;

        var stance = card.IsCanonical || card._owner == null || card.CombatState == null
            ? ChampModelDb.ChampStance<ChampNoStance>()
            : card.Owner.ChampStance();

        var locString = new LocString("champ_stances", $"{stance.GetType().GetPrefix()}{stance.Id.Entry}.finisher");
        stance.DynamicVars.AddTo(locString);

        yield return locString.GetFormattedText();
    }
}