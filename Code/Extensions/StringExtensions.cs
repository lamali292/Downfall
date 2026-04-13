using Downfall.Code.Character.Automaton;
using Downfall.Code.Character.Guardian;

namespace Downfall.Code.Extensions;

//Mostly utilities to get asset paths.
public static class StringExtensions
{
    public static string ImagePath(this string path)
    {
        return Path.Join(MainFile.ModId, "images", path);
    }

    public static string CardImagePath(this string path)
    {
        return Path.Join(MainFile.ModId, "images", "card_portraits", path);
    }

    private static string CardImageCharacterPath(this string path, string characterId)
    {
        return Path.Join(MainFile.ModId, "images", "card_portraits", characterId.ToLowerInvariant(), path);
    }

    private static string PowerImageCharacterPath(this string path, string characterId)
    {
        return Path.Join(MainFile.ModId, "images", "powers", characterId.ToLowerInvariant(), "small", path);
    }

    private static string BigPowerImageCharacterPath(this string path, string characterId)
    {
        return Path.Join(MainFile.ModId, "images", "powers", characterId.ToLowerInvariant(), "big", path);
    }


    public static string CardImageAutomatonPath(this string path)
        => path.CardImageCharacterPath(Automaton.CharacterId);

    public static string PowerImageAutomatonPath(this string path)
        => path.PowerImageCharacterPath(Automaton.CharacterId);

    public static string BigPowerImageAutomatonPath(this string path)
        => path.BigPowerImageCharacterPath(Automaton.CharacterId);
    
    
    public static string CardImageGuardianPath(this string path)
        => path.CardImageCharacterPath(Guardian.CharacterId);

    public static string PowerImageGuardianPath(this string path)
        => path.PowerImageCharacterPath(Guardian.CharacterId);

    public static string BigPowerImageGuardianPath(this string path)
        => path.BigPowerImageCharacterPath(Guardian.CharacterId);
    
    public static string BigCardImagePath(this string path)
    {
        return Path.Join(MainFile.ModId, "images", "card_portraits", "big", path);
    }

    public static string RemoveSuffix(this string s, string suffix)
    {
        var lastIndex = s.LastIndexOf('_');
        if (lastIndex < 0) return s;
        return string.Equals(s[(lastIndex + 1)..], suffix, StringComparison.OrdinalIgnoreCase)
            ? s[..lastIndex]
            : s;
    }

    public static string PowerImagePath(this string path)
    {
        return Path.Join(MainFile.ModId, "images", "powers", path);
    }

    public static string BigPowerImagePath(this string path)
    {
        return Path.Join(MainFile.ModId, "images", "powers", "big", path);
    }


    public static string RelicImagePath(this string path)
    {
        return Path.Join(MainFile.ModId, "images", "relics", path);
    }

    public static string BigRelicImagePath(this string path)
    {
        return Path.Join(MainFile.ModId, "images", "relics", "big", path);
    }

    public static string TresRelicImagePath(this string path)
    {
        return Path.Join(MainFile.ModId, "images", "relics", "tres", path);
    }

    public static string CharacterUiPath(this string path)
    {
        return Path.Join(MainFile.ModId, "images", "charui", path);
    }
}