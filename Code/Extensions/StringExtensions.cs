using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Extensions;

//Mostly utilities to get asset paths.
public static class StringExtensions
{
    private static string CardImageCharacterPath(this string path, string characterId)
    {
        return Path.Join(DownfallMainFile.ModId, "images", "card_portraits", characterId.ToLowerInvariant(), path);
    }


    private static string PowerImageCharacterPath(this string path, string characterId)
    {
        return path.DownfallPowerImagePath();
        //return Path.Join(DownfallMainFile.ModId, "images", "powers", characterId.ToLowerInvariant(), "small", path);
    }

    private static string BigPowerImageCharacterPath(this string path, string characterId)
    {
        return path.DownfallBigPowerImagePath();
        return Path.Join(DownfallMainFile.ModId, "images", "powers", characterId.ToLowerInvariant(), "big", path);
    }


    public static string DownfallPowerImagePath(this string path)
    {
        return Path.Join(DownfallMainFile.ModId, "images", "atlases", "power_atlas.sprites", path);
    }

    public static string DownfallBigPowerImagePath(this string path)
    {
        return Path.Join(DownfallMainFile.ModId, "images", "powers", path);
    }


    public static string CardImagePath<T>(this string path) where T : DownfallCharacterModel
    {
        return path.CardImageCharacterPath(ModelDb.Character<T>().CharId!);
    }


    public static string PowerImagePath<T>(this string path) where T : DownfallCharacterModel
    {
        return path.PowerImageCharacterPath(ModelDb.Character<T>().CharId!);
    }

    public static string BigPowerImagePath<T>(this string path) where T : DownfallCharacterModel
    {
        return path.BigPowerImageCharacterPath(ModelDb.Character<T>().CharId!);
    }

    public static string BigRelicImagePath(this string path)
    {
        return Path.Join(DownfallMainFile.ModId, "images", "relics", path);
    }

    public static string TresRelicImagePath(this string path)
    {
        return Path.Join(DownfallMainFile.ModId, "images", "atlases", "relic_atlas.sprites", path);
    }


    public static string RemoveSuffix(this string s, string suffix)
    {
        var lastIndex = s.LastIndexOf('_');
        if (lastIndex < 0) return s;
        return string.Equals(s[(lastIndex + 1)..], suffix, StringComparison.OrdinalIgnoreCase)
            ? s[..lastIndex]
            : s;
    }
}