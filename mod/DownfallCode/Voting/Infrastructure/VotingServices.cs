using Downfall.DownfallCode.Voting.Client;
using Godot;
using FileAccess = Godot.FileAccess;

namespace Downfall.DownfallCode.Voting;

/// <summary>
/// Composition root for the voting feature: wires the Godot-free transport and
/// session to their real (Godot) adapters. UI reaches the server through here.
/// </summary>
public static class VotingServices
{
    private const string BaseUrl = "https://api.downfall-sts2.org/voting";

    // Shared secret Caddy expects on every request to this domain. Not a
    // real per-user credential - same threat model as the old Supabase
    // publishable key: it just keeps casual scraping/abuse off the API.
    private const string GateKey = "0asdj0asdj0a0j0q22pm";

    public static VotingTransport Transport { get; } =
        new(new GodotHttpAdapter(), BaseUrl, GateKey, message => DownfallMainFile.Logger.Info(message));

    public static VotingSession Session { get; } =
        new(Transport, new GodotSessionStore(), new GodotUrlLauncher());

    public static VotingClient Client { get; } = new(Transport, Session, () => UserIdentity.Id);

    /// <summary>For fire-and-forget calls (votes, reports): the result has no UI, so just log a failure.</summary>
    public static async Task LogFailure(Task<VotingResult> call, string what)
    {
        var result = await call;
        if (!result.IsOk)
            GD.PrintErr($"{what} failed: {result.Error}");
    }

    private sealed class GodotUrlLauncher : IExternalLauncher
    {
        public void Open(string url) => OS.ShellOpen(url);
    }

    private sealed class GodotSessionStore : ISessionStore
    {
        private const string SessionFilePath = "user://voting_session.token";

        public string? Load()
        {
            if (!FileAccess.FileExists(SessionFilePath))
                return null;

            using var file = FileAccess.Open(SessionFilePath, FileAccess.ModeFlags.Read);
            var token = file?.GetAsText().Trim();
            return string.IsNullOrEmpty(token) ? null : token;
        }

        public void Save(string token)
        {
            using var file = FileAccess.Open(SessionFilePath, FileAccess.ModeFlags.Write);
            file?.StoreString(token);
        }

        public void Clear()
        {
            if (FileAccess.FileExists(SessionFilePath))
                DirAccess.RemoveAbsolute(SessionFilePath);
        }
    }
}
