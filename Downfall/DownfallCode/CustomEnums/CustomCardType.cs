using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;

namespace Downfall.DownfallCode.CustomEnums;


public static class CustomCardTypeRegistry
{
    private static readonly Dictionary<CardType, CardTypeProperties> Properties = new();

    public static string GetFramePath(CardType type)
    {
        return Properties[type].FramePath.Path;
    }

    public static string GetBorderPath(CardType type)
    {
        return Properties[type].BorderPath.Path;
    }

    public static void Register(CardType type, CardTypeProperties properties)
    {
        Properties[type] = properties;
    }
}

public record CardTypeProperties(CardBorderPath BorderPath, FramePath FramePath);

public class CardBorderPath(string path)
{
    public string Path { get; } = path;

    public static implicit operator CardBorderPath(string text)
    {
        return new CardBorderPath(text);
    }

    public static implicit operator CardBorderPath(CardType keyword)
    {
        return new CardBorderPath(ImageHelper.GetImagePath(
            $"atlases/ui_atlas.sprites/card/card_portrait_border_{keyword.ToString().ToLowerInvariant()}_s.tres"));
    }
}

public class FramePath(string path)
{
    public string Path { get; } = path;

    public static implicit operator FramePath(string text)
    {
        return new FramePath(text);
    }

    public static implicit operator FramePath(CardType keyword)
    {
        return new FramePath(
            $"atlases/ui_atlas.sprites/card/card_frame_{keyword.ToString().ToLowerInvariant()}_s.tres"
        );
    }
}

