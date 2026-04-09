using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Localization;

public class SkillBonusDescriptionSource : IExtraDescriptionSource
{
    private const string DownfallTable = "downfall";

    public IEnumerable<string> GetLines(CardModel card)
    {
        if (!card.Keywords.Contains(DownfallKeywords.TriggerSkillBonus)) yield break;
        yield return new LocString(DownfallTable, "TRIGGER_SKILL_BONUS.title").GetFormattedText();
        
    }
}