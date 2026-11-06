using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Hexaghost.HexaghostCode.CustomEnums;

public class HexaghostKeyword
{
    [CustomEnum] [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Retract;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Advance;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Afterlife;
}