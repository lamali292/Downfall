using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.DownfallCode.Utils;

public static class KeywordColorRegistry
{
    private static readonly Dictionary<CardKeyword, string> colors = new();

    public static void Register(CardKeyword keyword, string colorTag)
    {
        colors[keyword] = colorTag;
    }

    internal static bool TryGetColor(CardKeyword keyword, out string color)
    {
        return colors.TryGetValue(keyword, out color!);
    }
}