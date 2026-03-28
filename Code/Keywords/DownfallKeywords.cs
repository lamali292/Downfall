using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Keywords;

public static class DownfallKeywords
{
    [CustomEnum] [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Scry;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Encode;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Compile;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Cycle;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Status;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Insert;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Conjure;
    
    [CustomEnum] [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Chant;

    public static bool IsScry(this CardModel card)
    {
        return card.Keywords.Contains(Scry);
    }

    public static bool IsEncode(this CardModel card)
    {
        return card.Keywords.Contains(Encode);
    }

    public static bool IsCompile(this CardModel card)
    {
        return card.Keywords.Contains(Compile);
    }

    public static bool IsCycle(this CardModel card)
    {
        return card.Keywords.Contains(Cycle);
    }

    public static bool IsStatus(this CardModel card)
    {
        return card.Keywords.Contains(Status);
    }

    public static bool IsInsert(this CardModel card)
    {
        return card.Keywords.Contains(Insert);
    }

    public static bool IsConjure(this CardModel card)
    {
        return card.Keywords.Contains(Conjure);
    }
    
    public static bool isChant(this CardModel card)
    {
        return card.Keywords.Contains(Chant);
    }
}