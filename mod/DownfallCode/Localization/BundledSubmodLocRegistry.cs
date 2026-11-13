namespace Downfall.DownfallCode.Localization;

/// <summary>
///     When every submod is bundled into a single Downfall.dll/Downfall.pck
///     (see DownfallAllInOne), only one Mod entry ("Downfall") ends up registered
///     with ModManager, so the base game's ModManager.GetModdedLocTables never
///     checks res://{submodId}/localization/... for any of the bundled submods.
///     Each submod calls Register(ModId) from its own Initialize() to announce
///     that its localization folder should be checked too. See
///     GetModdedLocTablesPatch for where this list is consumed.
/// </summary>
public static class BundledSubmodLocRegistry
{
    private static readonly HashSet<string> _ids = new();

    public static IReadOnlyCollection<string> Ids => _ids;

    public static void Register(string modId)
    {
        _ids.Add(modId);
    }
}