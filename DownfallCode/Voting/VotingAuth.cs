using Godot;
using FileAccess = Godot.FileAccess;

namespace Downfall.DownfallCode.Voting;

/// <summary>
/// Steam-verified session for uploading art submissions. Separate from
/// <see cref="UserIdentity"/>, which is only a hashed local SteamID used for
/// anonymous voting - upload requires a real, server-verified steamid64 so
/// abusive uploaders can be banned.
/// </summary>
public static class VotingAuth
{
    private const string SessionFilePath = "user://voting_session.token";
    private const int PollIntervalMs = 2000;
    private const int TimeoutMs = 120_000;

    public static string? Token { get; private set; } = Load();

    public static bool IsSignedIn => Token != null;

    /// <summary>
    /// Signs in if there's no session yet (opening the Steam login page),
    /// otherwise returns immediately. Used by every write endpoint that now
    /// requires a Steam-verified identity (voting, flagging, uploading) so
    /// callers don't each have to repeat the "am I signed in" dance.
    /// </summary>
    public static async Task<bool> EnsureSignedIn()
    {
        if (IsSignedIn)
            return true;

        var (ok, _) = await LoginAsync();
        return ok;
    }

    public static async Task<(bool ok, string status)> LoginAsync()
    {
        var (state, loginUrl) = await VotingApi.Instance.StartSteamLogin();
        if (state == null || loginUrl == null)
            return (false, VotingUi.Loc("DOWNFALL-VOTING.error_login_unreachable"));

        OS.ShellOpen(loginUrl);

        var elapsed = 0;
        while (elapsed < TimeoutMs)
        {
            await Task.Delay(PollIntervalMs);
            elapsed += PollIntervalMs;

            var (status, token) = await VotingApi.Instance.PollSteamLogin(state);

            switch (status)
            {
                case "completed" when token != null:
                    Token = token;
                    Save(token);
                    return (true, VotingUi.Loc("DOWNFALL-VOTING.status_login_success"));
                case "banned":
                    return (false, VotingUi.Loc("DOWNFALL-VOTING.error_login_banned"));
                case "expired":
                    return (false, VotingUi.Loc("DOWNFALL-VOTING.error_login_expired"));
                case "error":
                    return (false, VotingUi.Loc("DOWNFALL-VOTING.error_login_generic"));
                // "pending" / "unknown": keep polling.
            }
        }

        return (false, VotingUi.Loc("DOWNFALL-VOTING.error_login_timeout"));
    }

    public static void Logout()
    {
        Token = null;
        if (FileAccess.FileExists(SessionFilePath))
            DirAccess.RemoveAbsolute(SessionFilePath);
    }

    private static string? Load()
    {
        if (!FileAccess.FileExists(SessionFilePath))
            return null;

        using var file = FileAccess.Open(SessionFilePath, FileAccess.ModeFlags.Read);
        var token = file?.GetAsText().Trim();
        return string.IsNullOrEmpty(token) ? null : token;
    }

    private static void Save(string token)
    {
        using var file = FileAccess.Open(SessionFilePath, FileAccess.ModeFlags.Write);
        file?.StoreString(token);
    }
}
