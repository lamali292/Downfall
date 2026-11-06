using BaseLib.Abstracts;
using BaseLib.Patches.Content;

namespace Downfall.DownfallCode.Utils;

/// <summary>
///     Every character mod announces its position on the character select screen and in the compendium with a
///     plain number, so no mod has to reference another mod's character type. Lower numbers come first. Use gaps
///     (10, 20, ...) so a new character can slot in between existing ones. Call <see cref="Register{T}" /> from
///     the mod's initializer; characters that are never registered (hidden or superseded submods) are simply not
///     part of the order.
///     <para>
///         <see cref="Apply" /> runs once after the models exist (<see cref="PostInitRegistry" />) and reorders
///         <see cref="CustomContentDictionary.CustomCharacters" /> directly. BaseLib's own
///         <c>TryOrderCustomCharacters</c> is not used: it must be called before <c>ModelDb.Init</c>, sorts
///         characters it was not told about to the front, and re-sorts the whole list once per declared order.
///         Here the registered characters are only rearranged among the slots they already occupy, so characters
///         from other mods keep their position.
///     </para>
/// </summary>
public static class CharacterOrderRegistry
{
    private static readonly Dictionary<Type, int> _orders = new();

    public static void Register<T>(int order) where T : CustomCharacterModel
    {
        _orders[typeof(T)] = order;
    }

    internal static void Apply()
    {
        var characters = CustomContentDictionary.CustomCharacters;
        var slots = Enumerable.Range(0, characters.Count)
            .Where(i => _orders.ContainsKey(characters[i].GetType()))
            .ToList();
        var sorted = slots.Select(i => characters[i])
            .OrderBy(c => _orders[c.GetType()])
            .ToList();
        for (var n = 0; n < slots.Count; n++)
            characters[slots[n]] = sorted[n];
        DownfallMainFile.Logger.Info(
            $"Character order: {string.Join(", ", characters.Select(c => c.GetType().Name))}");
    }
}
