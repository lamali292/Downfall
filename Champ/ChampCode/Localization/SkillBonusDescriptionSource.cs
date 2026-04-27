using Champ.ChampCode.CustomEnums;
using Downfall.DownfallCode.Localization;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Localization;

public class SkillBonusDescriptionSource : IExtraDescriptionSource
{
    private const string DownfallTable = "downfall";

    public IEnumerable<string> GetLines(CardModel card)
    {
        if (!card.Keywords.Contains(ChampKeyword.TriggerSkillBonus)) yield break;
        yield return new LocString(DownfallTable, "TRIGGER_SKILL_BONUS.title").GetFormattedText();
    }
}