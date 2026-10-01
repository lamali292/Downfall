namespace Downfall.DownfallCode.Voting.Client;

public enum VotingErrorCode
{
    /// <summary>No anonymous identity available (Steam not running).</summary>
    NoIdentity,

    /// <summary>The call needs a session and none exists (and it does not sign in by itself).</summary>
    NotSignedIn,

    LoginUnreachable,
    LoginBanned,
    LoginExpired,
    LoginFailed,
    LoginTimeout,

    /// <summary>HTTP 401 - the session was rejected.</summary>
    SessionExpired,

    /// <summary>HTTP 403 - the account is banned.</summary>
    Banned,

    /// <summary>HTTP 413.</summary>
    TooLarge,

    UnsupportedImageType,

    /// <summary>No response at all (status 0).</summary>
    Network,

    /// <summary>A 2xx response that did not have the expected shape.</summary>
    BadResponse,

    /// <summary>Any other non-2xx; see <see cref="VotingError.Status"/> and <see cref="VotingError.ServerMessage"/>.</summary>
    Server
}

public readonly record struct VotingError(VotingErrorCode Code, long Status = 0, string? ServerMessage = null);

public readonly record struct VotingResult(VotingError? Error)
{
    public bool IsOk => Error == null;

    public static VotingResult Ok => new(null);

    public static implicit operator VotingResult(VotingError error) => new(error);
}

public readonly record struct VotingResult<T>(T? Value, VotingError? Error)
{
    public bool IsOk => Error == null;

    public static implicit operator VotingResult<T>(VotingError error) => new(default, error);

    public static implicit operator VotingResult<T>(T value) => new(value, null);
}

public sealed record RemoteEntry(
    long Id,
    string Category,
    string Entry,
    string ImageUrl,
    string Author,
    int Upvotes,
    bool Liked,
    IReadOnlySet<string> MyFlags,
    long SubmittedAt);

/// <summary>NextOffset is null once the feed is exhausted.</summary>
public sealed record FeedPage(IReadOnlyList<RemoteEntry> Items, int? NextOffset);

public sealed record RemoteCard(string Category, string Entry);

public sealed record RemoteMySubmission(
    long Id,
    string? ImageUrl,
    string Status,
    string Category,
    string Entry,
    int Upvotes,
    long SubmittedAt);

/// <summary>Both nullable: the account may not have set a credit name yet.</summary>
public sealed record MyProfile(string? CreditName, string? SteamId);
