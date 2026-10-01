using System.Text;

namespace Downfall.DownfallCode.Voting.Client;

/// <summary>
/// The one place that touches the network: real HTTP in the game, a scripted
/// fake in tests. Implementations never throw for failed requests, they return
/// status 0.
/// </summary>
public interface IHttpAdapter
{
    Task<VotingResponse> SendAsync(HttpVerb method, string url, IReadOnlyList<string> headers, byte[] body);
}

/// <summary>
/// What the transport needs from whoever owns the session: the current bearer
/// token, and a way to get a fresh one after the server rejected it. Declared
/// here (not on the session) so the transport does not depend on the session.
/// </summary>
public interface IAuthTokens
{
    string? Token { get; }

    Task<bool> ReauthenticateAsync();
}

/// <summary>
/// Sends <see cref="VotingRequest"/>s: adds the gate key and bearer token,
/// logs, and performs the single re-authenticate-and-retry on a 401. Knows
/// nothing of Godot, sign-in policy or localized text.
/// </summary>
public sealed class VotingTransport(
    IHttpAdapter http,
    string baseUrl,
    string gateKey,
    Action<string>? log = null)
{
    // Requests/responses that list every card (submissions/feed) can run
    // into the tens of KB; logging them in full just to see "-> POST .../vote"
    // elsewhere buries the log in noise, so anything past this gets cut off.
    private const int LogBodyLimit = 300;

    /// <summary>
    /// Set once by whoever owns the session; a property rather than a
    /// constructor argument because the session itself is built on top of
    /// this transport.
    /// </summary>
    public IAuthTokens? Auth { get; set; }

    public async Task<VotingResponse> SendAsync(VotingRequest request)
    {
        var response = await SendOnce(request, request.Authed ? Auth?.Token : null);

        if (request is { Authed: true, RetryOnUnauthorized: true } &&
            response.Status == 401 &&
            Auth != null &&
            await Auth.ReauthenticateAsync())
        {
            response = await SendOnce(request, Auth.Token);
        }

        return response;
    }

    private async Task<VotingResponse> SendOnce(VotingRequest request, string? token)
    {
        var url = baseUrl + request.Path;
        var body = request.Body ?? [];

        // Headers are a simple list (not a dictionary) because that is what
        // the Godot adapter needs; order is irrelevant to the server.
        var headers = new List<string>
        {
            // Shared secret Caddy expects on every request to this domain.
            // Not a per-user credential: it just keeps casual scraping off.
            $"apikey: {gateKey}",
            $"Content-Type: {request.ContentType}"
        };

        if (token != null)
            headers.Add($"Authorization: Bearer {token}");

        var shownBody = request.ContentType == "application/json"
            ? Encoding.UTF8.GetString(body)
            : $"<{body.Length} bytes>";
        log?.Invoke($"[VotingApi] -> {request.Method} {url} {Truncate(shownBody)}");

        var response = await http.SendAsync(request.Method, url, headers, body);

        log?.Invoke($"[VotingApi] <- {response.Status} {url} :: {Truncate(response.Body)}");
        return response;
    }

    private static string Truncate(string text) =>
        text.Length > LogBodyLimit ? $"{text[..LogBodyLimit]}... ({text.Length} chars)" : text;
}
