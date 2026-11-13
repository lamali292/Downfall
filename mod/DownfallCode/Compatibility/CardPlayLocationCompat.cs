using MegaCrit.Sts2.Core.Models;
using Downfall.DownfallCode.Patches;

namespace Downfall.DownfallCode.Compatibility;

/// <summary>
///     The one place that knows how "where does a played card go" differs between game versions:
///     the new game returns a <c>CardLocation</c> struct (with a Player), the old game a plain
///     (PileType, CardPilePosition) tuple with no Player. Mechanics never branch on the version:
///     <list type="bullet">
///         <item>
///             To <b>modify</b> the final destination, implement <see cref="IModifyCardPlayResultLocation" />
///             (dispatched after vanilla listeners, on both versions).
///         </item>
///         <item>
///             To change what vanilla listeners (Rebound, ...) <b>see</b> before they run, register a filter with
///             <see cref="RegisterInitialLocationFilter" />.
///         </item>
///     </list>
///     The version-specific Harmony patches behind both are chosen once, in <see cref="PatchTypes" />.
/// </summary>
public static class CardPlayLocationCompat
{
    private static readonly List<Func<CardModel, CardLocationCompatiblity, CardLocationCompatiblity>> Filters = [];

    /// <summary>
    ///     False on the old game: it has no Player in a card location, so a redirect to another
    ///     player's pile is dropped and only same-owner destinations take effect.
    /// </summary>
    public static bool SupportsCrossPlayerRedirect => GameVersion.HasCardLocation;

    /// <summary>The Harmony patch classes that implement this module for the running game version.</summary>
    public static IEnumerable<Type> PatchTypes => GameVersion.HasCardLocation
        ?
        [
            typeof(CardPlayInitialLocationNewPatch),
            typeof(ModifyCardPlayResultLocationNewPatch),
            typeof(AfterModifyingLocationNewPatch)
        ]
        :
        [
            typeof(CardPlayInitialLocationOldPatch),
            typeof(ModifyCardPlayResultLocationOldPatch),
            typeof(AfterModifyingLocationOldPatch)
        ];

    /// <summary>
    ///     Registers a filter applied to the location a card play resolves to <i>before</i> vanilla
    ///     listeners (Rebound, ...) inspect it. Register once at mod initialization.
    /// </summary>
    public static void RegisterInitialLocationFilter(
        Func<CardModel, CardLocationCompatiblity, CardLocationCompatiblity> filter)
    {
        Filters.Add(filter);
    }

    internal static CardLocationCompatiblity ApplyInitialFilters(CardModel card, CardLocationCompatiblity location)
    {
        foreach (var filter in Filters) location = filter(card, location);
        return location;
    }
}
