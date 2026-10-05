using MegaCrit.Sts2.Core.Modding;

namespace Downfall.DownfallCode.Utils;

/// <summary>
/// Generic submod supersession guard (ADR 0003). A bundled standalone submod's
/// <c>MainFile.Initialize()</c> calls <see cref="IsSupersededBy"/> first and returns
/// early, skipping all its own registration, if any of its declared replacement
/// ModIds is also loaded (e.g. stock SlimeBoss stepping aside for "SlimeBoss Beta").
/// </summary>
public static partial class ReplaceableSubmod
{
    /// <summary>
    /// True if any of <paramref name="replacementModIds"/> is currently loaded.
    /// Thin wrapper around <see cref="ModManager.GetLoadedMods"/> - the actual
    /// decision is the pure <see cref="IsSupersededByAny"/> (see the other partial
    /// of this class), which is unit-tested without a game context.
    /// </summary>
    public static bool IsSupersededBy(params string[] replacementModIds)
    {
        var loadedModIds = ModManager.GetLoadedMods().Select(m => m.manifest?.id);
        return IsSupersededByAny(loadedModIds, replacementModIds);
    }
}
