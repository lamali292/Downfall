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
    /// True if any of <paramref name="replacementModIds"/> is present and will load.
    /// Deliberately reads <see cref="ModManager.Mods"/> (every discovered mod), not
    /// <see cref="ModManager.GetLoadedMods"/> (state == Loaded only): ModManager runs
    /// every mod's <c>Initialize()</c> in dependency order, and a replacement mod must
    /// depend on Downfall (it needs Downfall's types) to load after it - so at the
    /// point Downfall's own Initialize() calls this, the replacement's state is still
    /// None, never Loaded yet. ModManager.RemoveDisabledMods() runs before any
    /// Initialize() call, though, so by then every mod's Disabled/DisabledDuplicate
    /// status is already final - checking "discovered and not disabled" here is both
    /// early enough to matter and correct. The actual decision is the pure
    /// <see cref="IsSupersededByAny"/> (see the other partial of this class), which is
    /// unit-tested without a game context.
    /// </summary>
    public static bool IsSupersededBy(params string[] replacementModIds)
    {
        var candidateModIds = ModManager.Mods
            .Where(m => m.state != ModLoadState.Disabled && m.state != ModLoadState.DisabledDuplicate)
            .Select(m => m.manifest?.id);
        return IsSupersededByAny(candidateModIds, replacementModIds);
    }
}
