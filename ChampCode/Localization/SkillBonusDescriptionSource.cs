using Champ.ChampCode.CustomEnums;
using Downfall.DownfallCode.Localization;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Localization;

public class SkillBonusDescriptionSource : IExtraDescriptionSource
{
    public IEnumerable<string> GetLines(CardModel card)
    {
        if (!card.Keywords.Contains(ChampKeyword.TriggerSkillBonus)) yield break;
        yield return new LocString("static_hover_tips", "TRIGGER_SKILL_BONUS.title").GetFormattedText();
    }
}