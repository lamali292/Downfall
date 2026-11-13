using System.Text.Json;

namespace Downfall.DownfallCode.Voting.Client;

/// <summary>Persists the session token between game launches.</summary>
public interface ISessionStore
{
    string? Load();

    void Save(string token);

    void Clear();
}

/// <summary>Opens a URL in the player's browser.</summary>
public interface IExternalLauncher
{
    void Open(string url);
}

public enum LoginOutcome
{
    Success,
    Unreachable,
    Banned,
    Expired,
    Failed,
    Timeout
}

/// <summary>
/// Steam-verified session for votes, reports and uploads: owns the token and
/// the browser sign-in (open the login page, poll until the server reports it
/// done). Separate from the anonymous hashed id used for browsing. Depends on
/// the transport only; the transport reaches back through
/// <see cref="IAuthTokens"/>, never this class. Returns outcome codes, never
/// localized text.
/// </summary>
public sealed class VotingSession : IAuthTokens
{
    private readonly VotingTransport _transport;
    private readonly ISessionStore _store;
    private readonly IExternalLauncher _launcher;
    private readonly Func<int, Task> _delay;
    private readonly int _pollIntervalMs;
    private readonly int _timeoutMs;

    // Concurrent callers (e.g. two quick votes while signed out) share one
    // browser sign-in instead of each opening their own.
    private Task<LoginOutcome>? _login;

    public VotingSession(
        VotingTransport transport,
        ISessionStore store,
        IExternalLauncher launcher,
        Func<int, Task>? delay = null,
        int pollIntervalMs = 2000,
        int timeoutMs = 120_000)
    {
        _transport = transport;
        _store = store;
        _launcher = launcher;
        _delay = delay ?? Task.Delay;
        _pollIntervalMs = pollIntervalMs;
        _timeoutMs = timeoutMs;

        Token = store.Load();
        transport.Auth = this;
    }

    public string? Token { get; private set; }

    public bool IsSignedIn => Token != null;

    /// <summary>
    /// Fired right after a successful sign-in, so UI that opened before it
    /// happened (e.g. the upload popup) can refresh.
    /// </summary>
    public event Action? SignedIn;

    /// <summary>Signs in only if there is no session yet.</summary>
    public Task<LoginOutcome> EnsureSignedInAsync() =>
        IsSignedIn ? Task.FromResult(LoginOutcome.Success) : LoginAsync();

    public Task<LoginOutcome> LoginAsync()
    {
        if (_login is { IsCompleted: false })
            return _login;

        return _login = RunLogin();
    }

    /// <summary>
    /// Clears a session the server just rejected (401) and starts a fresh
    /// sign-in, so a stale token recovers with one more browser prompt.
    /// </summary>
    public async Task<bool> ReauthenticateAsync()
    {
        Logout();
        return await LoginAsync() == LoginOutcome.Success;
    }

    public void Logout()
    {
        Token = null;
        _store.Clear();
    }

    private async Task<LoginOutcome> RunLogin()
    {
        var start = await _transport.SendAsync(new VotingRequest(HttpVerb.Post, "/auth/steam/start")
        {
            Body = "{}"u8.ToArray()
        });

        if (!start.IsSuccess || !TryReadStart(start.Body, out var state, out var loginUrl))
            return LoginOutcome.Unreachable;

        _launcher.Open(loginUrl);

        var elapsed = 0;
        while (elapsed < _timeoutMs)
        {
            await _delay(_pollIntervalMs);
            elapsed += _pollIntervalMs;

            var poll = await _transport.SendAsync(new VotingRequest(
                HttpVerb.Get, $"/auth/steam/poll?state={Uri.EscapeDataString(state)}"));

            if (poll.Status == 403)
                return LoginOutcome.Banned;

            if (!poll.IsSuccess || !TryReadPoll(poll.Body, out var status, out var token))
                return LoginOutcome.Failed;

            switch (status)
            {
                case "completed" when token != null:
                    Token = token;
                    _store.Save(token);
                    SignedIn?.Invoke();
                    return LoginOutcome.Success;
                case "banned":
                    return LoginOutcome.Banned;
                case "expired":
                    return LoginOutcome.Expired;
                case "error":
                    return LoginOutcome.Failed;
                // "pending" / "unknown": keep polling.
            }
        }

        return LoginOutcome.Timeout;
    }

    private static bool TryReadStart(string json, out string state, out string loginUrl)
    {
        state = loginUrl = "";
        try
        {
            using var doc = JsonDocument.Parse(json);
            state = doc.RootElement.GetProperty("state").GetString() ?? "";
            loginUrl = doc.RootElement.GetProperty("loginUrl").GetString() ?? "";
            return state.Length > 0 && loginUrl.Length > 0;
        }
        catch (Exception e) when (e is JsonException or KeyNotFoundException or InvalidOperationException)
        {
            return false;
        }
    }

    private static bool TryReadPoll(string json, out string status, out string? token)
    {
        status = "";
        token = null;
        try
        {
            using var doc = JsonDocument.Parse(json);
            status = doc.RootElement.GetProperty("status").GetString() ?? "";
            if (status == "completed")
                token = doc.RootElement.GetProperty("token").GetString();
            return true;
        }
        catch (Exception e) when (e is JsonException or KeyNotFoundException or InvalidOperationException)
        {
            return false;
        }
    }
}
