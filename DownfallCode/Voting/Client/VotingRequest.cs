namespace Downfall.DownfallCode.Voting.Client;

public enum HttpVerb
{
    Get,
    Post,
    Put,
    Delete
}

/// <summary>
/// One call to the voting server, described in plain values so it can be
/// sent by the real HTTP adapter or a scripted fake. <see cref="Path"/> is
/// relative to the server's base URL (leading slash included).
/// </summary>
public sealed record VotingRequest(HttpVerb Method, string Path)
{
    public byte[]? Body { get; init; }

    public string ContentType { get; init; } = "application/json";

    /// <summary>Sends the session's bearer token; without it the call is anonymous.</summary>
    public bool Authed { get; init; }

    /// <summary>
    /// On a 401, have the session re-authenticate and resend the request
    /// once. Only meaningful together with <see cref="Authed"/>. On by
    /// default; a call that must never start an interactive login (the
    /// profile read) opts out.
    /// </summary>
    public bool RetryOnUnauthorized { get; init; } = true;
}

/// <summary>
/// Status 0 means the request never got a response (could not be sent, or no
/// network); <see cref="Body"/> is then a short diagnostic, not server output.
/// </summary>
public readonly record struct VotingResponse(long Status, string Body)
{
    public bool IsSuccess => Status is >= 200 and < 300;
}
