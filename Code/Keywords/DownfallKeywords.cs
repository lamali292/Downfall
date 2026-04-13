using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Keywords;

public class DownfallKeywords
{
    [CustomEnum] 
    public static CardKeyword TriggerSkillBonus;
    
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)] 
    public static CardKeyword Pyre;
}