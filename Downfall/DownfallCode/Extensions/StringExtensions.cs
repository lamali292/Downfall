using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Extensions;

//Mostly utilities to get asset paths.
public static class StringExtensions
{
    public static string DownfallPowerImagePath(this string path)
    {
        return Path.Join(DownfallMainFile.ModId, "images", "atlases", "power_atlas.sprites", path);
    }

    public static string DownfallBigPowerImagePath(this string path)
    {
        return Path.Join(DownfallMainFile.ModId, "images", "powers", path);
    }

    public static string CardImageAtlasPath<T>(this string path) where T : DownfallCharacterModel
    {
        var character = ModelDb.Character<T>();
        var modId     = character.ModId;
        return Path.Join(modId, "images", "atlases", "card_atlas.sprites", path);
    }

    
    public static string RestSitePath(this string path)
    {
        return Path.Join(DownfallMainFile.ModId, "images", "ui", "restsite", path);
    }


    public static string EnchantmentPath<T>(this string path) where T : DownfallCharacterModel
    {
        var modId = ModelDb.Character<T>().ModId;
        return Path.Join(modId, "images", "enchantments", path);
    }


    public static string PowerImagePath<T>(this string path) where T : DownfallCharacterModel
    {
        var modId = ModelDb.Character<T>().ModId;
        return Path.Join(modId, "images", "atlases", "power_atlas.sprites", path);
    }

    public static string BigPowerImagePath<T>(this string path) where T : DownfallCharacterModel
    {
        var modId = ModelDb.Character<T>().ModId;
        return Path.Join(modId, "images", "powers", path);
    }

    public static string BigRelicImagePath<T>(this string path) where T : DownfallCharacterModel
    {
        var modId = ModelDb.Character<T>().ModId;
        return Path.Join(modId, "images", "relics", path);
    }
    public static string TresRelicImagePath<T>(this string path) where T : DownfallCharacterModel
    {
        var modId = ModelDb.Character<T>().ModId;
        return Path.Join(modId, "images", "atlases", "relic_atlas.sprites", path);
    }
}