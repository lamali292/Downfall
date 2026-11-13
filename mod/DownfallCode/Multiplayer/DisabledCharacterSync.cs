using BaseLib.Config;
using Downfall.DownfallCode.Config;
using Godot;

namespace Downfall.DownfallCode.Multiplayer;

/// <summary>
/// Which characters a player has disabled decides which models exist, and therefore the multiplayer model-ID hash.
/// The handshake only carries mod names, so the disabled set rides along as one extra non-gameplay "mod" entry; a
/// client that fails to join because of a hash mismatch can then read the host's set and offer to adopt it.
/// </summary>
internal static class DisabledCharacterSync
{
    private const string MarkerPrefix = "DownfallDisabled:";
    private const string ConnectLobbyArg = "+connect_lobby";
    private const string SyncedArg = "--downfall-synced";

    internal static readonly string[] Characters =
    [
        "Automaton", "Awakened", "Champ", "Guardian", "Hermit", "Hexaghost", "SlimeBoss", "Snecko"
    ];

    /// <summary>What was disabled when the game started: the config can change afterwards, the loaded models cannot.</summary>
    private static List<string>? _startupDisabled;

    /// <summary>Steam lobby of the join attempt that just failed its handshake, if it was a Steam join.</summary>
    internal static ulong? FailedJoinLobbyId { get; set; }

    /// <summary>
    /// True in the process started right after applying the host's settings. If the hashes still differ there, the
    /// cause is not the character set, so the game's plain error is shown instead of offering another restart.
    /// </summary>
    internal static bool JustSynced => OS.GetCmdlineArgs().Contains(SyncedArg);

    internal static void CaptureStartupState()
    {
        _startupDisabled = Characters.Where(DownfallConfig.IsDisabled).ToList();
    }

    internal static List<string> LocalDisabled()
    {
        return _startupDisabled ?? Characters.Where(DownfallConfig.IsDisabled).ToList();
    }

    internal static bool IsMarker(string mod) => mod.StartsWith(MarkerPrefix, StringComparison.Ordinal);

    internal static string BuildMarker(IEnumerable<string> disabled)
    {
        return MarkerPrefix + string.Join(",", disabled.OrderBy(static c => c, StringComparer.Ordinal));
    }

    /// <returns>false when the peer does not advertise a disabled set (older build), so nothing can be compared.</returns>
    internal static bool TryParse(IEnumerable<string>? mods, out List<string> disabled)
    {
        disabled = [];
        var marker = mods?.FirstOrDefault(IsMarker);
        if (marker == null) return false;

        disabled = marker[MarkerPrefix.Length..]
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Where(static c => Characters.Contains(c))
            .OrderBy(static c => c, StringComparer.Ordinal)
            .ToList();
        return true;
    }

    internal static void Apply(IReadOnlyCollection<string> disabled)
    {
        foreach (var character in Characters)
            DownfallConfig.SetDisabled(character, disabled.Contains(character));
        ModConfigRegistry.Get<DownfallConfig>()?.Save();
    }

    /// <summary>
    /// Starts a fresh game process with the same arguments, then quits this one. The game itself joins the lobby named
    /// by <c>+connect_lobby</c> right after launch (the path Steam invites use).
    /// </summary>
    internal static void RestartGame(ulong? rejoinLobbyId)
    {
        var args = OS.GetCmdlineArgs().ToList();
        var old = args.IndexOf(ConnectLobbyArg);
        if (old >= 0) args.RemoveRange(old, Math.Min(2, args.Count - old));
        args.Remove(SyncedArg);
        args.Add(SyncedArg);
        if (rejoinLobbyId.HasValue) args.AddRange([ConnectLobbyArg, rejoinLobbyId.Value.ToString()]);

        var pid = OS.CreateProcess(OS.GetExecutablePath(), args.ToArray());
        if (pid == -1)
            DownfallMainFile.Logger.Error("[CharacterSync] Could not relaunch the game; quitting so it can be started manually.");
        (Engine.GetMainLoop() as SceneTree)?.Quit();
    }
}
