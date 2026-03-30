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
        return Path.Join(DownfallMainFile.ModId, "images", "powers", characterId.ToLowerInvariant(), "small", path);
    }

    private static string BigPowerImageCharacterPath(this string path, string characterId)
    {
        return Path.Join(DownfallMainFile.ModId, "images", "powers", characterId.ToLowerInvariant(), "big", path);
    }


    public static string DownfallPowerImagePath(this string path)
    {
        return Path.Join(DownfallMainFile.ModId, "images", "powers", "downfall", "small", path);
    }

    public static string DownfallBigPowerImagePath(this string path)
    {
        return Path.Join(DownfallMainFile.ModId, "images", "powers", "downfall", "big", path);
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
    
    private static string BigRelicImageCharacterPath(this string path, string characterId)
    {
        return Path.Join(DownfallMainFile.ModId, "images", "relics", characterId.ToLowerInvariant(), "big", path);
    }
    
    private static string TresRelicImageCharacterPath(this string path, string characterId)
    {
        return Path.Join(DownfallMainFile.ModId, "images", "relics", characterId.ToLowerInvariant(), "tres", path);
    }

    
    public static string BigRelicImagePath(this string path)
    {
        return Path.Join(DownfallMainFile.ModId, "images", "relics", "big", path);

    }

    public static string TresRelicImagePath(this string path)
    {
        return Path.Join(DownfallMainFile.ModId, "images", "relics", "tres", path);
    }
    
    public static string BigRelicImagePath<T>(this string path) where T : DownfallCharacterModel
    {
           return path.BigRelicImageCharacterPath(ModelDb.Character<T>().CharId!);
    }

    public static string TresRelicImagePath<T>(this string path) where T : DownfallCharacterModel
    {
        return path.TresRelicImageCharacterPath(ModelDb.Character<T>().CharId!);
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