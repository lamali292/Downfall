using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace SlimeBoss.SlimeBossCode.CustomEnums;


public class SlimeBossKeyword
{
    [CustomEnum] public static CardKeyword Slurp;
    [CustomEnum] public static CardKeyword Command;
    [CustomEnum] [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Buried;
}