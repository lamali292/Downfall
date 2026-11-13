using System.Text;
using System.Text.Json;

namespace Downfall.DownfallCode.Voting.Client;

/// <summary>
/// Typed interface to the voting server: feed, votes, reports, profile,
/// uploads and own submissions. Signs in (via the session) before the writes
/// that need a Steam-verified identity. Returns typed results or error codes,
/// never player-facing strings and never throws for failed requests.
/// </summary>
public sealed class VotingClient(VotingTransport transport, VotingSession session, Func<string?> anonymousId)
{
    private static string Esc(string s) => Uri.EscapeDataString(s);

    private static byte[] Json(object body) => JsonSerializer.SerializeToUtf8Bytes(body);

    // ---- Anonymous browsing ----

    /// <summary>
    /// One page of the server-sorted feed across all cards. <paramref name="pools"/>
    /// are pool names mapped server-side to mod prefixes (empty = no filter).
    /// </summary>
    public async Task<VotingResult<FeedPage>> GetFeedAsync(
        IReadOnlyCollection<string> pools, string sort, int offset, int limit)
    {
        var user = anonymousId();
        if (user == null)
            return new VotingError(VotingErrorCode.NoIdentity);

        var body = new Dictionary<string, object>
        {
            ["user"] = user,
            ["sort"] = sort,
            ["offset"] = offset,
            ["limit"] = limit,
        };

        if (pools.Count > 0)
            body["pools"] = pools.ToArray();

        var response = await transport.SendAsync(new VotingRequest(HttpVerb.Post, "/submissions/feed") { Body = Json(body) });
        return Parse(response, VotingParser.ParseFeed);
    }

    /// <summary>Every card currently open for art submission (admin-curated server-side).</summary>
    public async Task<VotingResult<IReadOnlyList<RemoteCard>>> GetMissingCardsAsync()
    {
        var response = await transport.SendAsync(new VotingRequest(HttpVerb.Get, "/missing-cards"));
        var parsed = Parse(response, VotingParser.ParseMissingCards);
        return parsed.IsOk
            ? new VotingResult<IReadOnlyList<RemoteCard>>(parsed.Value, null)
            : parsed.Error!.Value;
    }

    // ---- Votes and reports (Steam-verified; signs in when needed, retries once on 401) ----

    public Task<VotingResult> CastVoteAsync(long submissionId) =>
        WriteAsync("/vote", new { submissionId });

    public Task<VotingResult> ClearVoteAsync(long submissionId) =>
        WriteAsync("/unvote", new { submissionId });

    /// <summary>All reason changes of one "Report" submit in a single request.</summary>
    public Task<VotingResult> SetFlagsAsync(
        long submissionId, IReadOnlyCollection<string> add, IReadOnlyCollection<string> remove) =>
        add.Count == 0 && remove.Count == 0
            ? Task.FromResult(VotingResult.Ok)
            : WriteAsync("/flag/batch", new { submissionId, add, remove });

    private async Task<VotingResult> WriteAsync(string path, object body)
    {
        if (await RequireSignIn() is { } error)
            return error;

        var response = await transport.SendAsync(new VotingRequest(HttpVerb.Post, path)
        {
            Body = Json(body),
            Authed = true,
        });

        return ToResult(response);
    }

    // ---- Account (no sign-in prompt for reads) ----

    /// <summary>Credit name and verified steamid64 of the signed-in account.</summary>
    public async Task<VotingResult<MyProfile>> GetMyProfileAsync()
    {
        if (!session.IsSignedIn)
            return new VotingError(VotingErrorCode.NotSignedIn);

        var response = await transport.SendAsync(new VotingRequest(HttpVerb.Get, "/my/profile")
        {
            Authed = true,
            // A stale token must not pop an interactive browser login from a read.
            RetryOnUnauthorized = false,
        });
        return Parse(response, VotingParser.ParseProfile);
    }

    /// <summary>
    /// Saves the account's art-credit name (applies to every submission of the
    /// account). Rename is server-rate-limited; that arrives as
    /// <see cref="VotingErrorCode.Server"/> with the server's message.
    /// </summary>
    public async Task<VotingResult> SetCreditNameAsync(string creditName)
    {
        if (await RequireSignIn() is { } error)
            return error;

        var response = await transport.SendAsync(new VotingRequest(HttpVerb.Put, "/my/profile")
        {
            Body = Json(new { creditName }),
            Authed = true,
        });

        return ToResult(response);
    }

    // ---- Own submissions ----

    public async Task<VotingResult<IReadOnlyList<RemoteMySubmission>>> GetMySubmissionsAsync()
    {
        if (await RequireSignIn() is { } error)
            return error;

        var response = await transport.SendAsync(new VotingRequest(HttpVerb.Get, "/my/submissions") { Authed = true });
        var parsed = Parse(response, VotingParser.ParseMySubmissions);
        return parsed.IsOk
            ? new VotingResult<IReadOnlyList<RemoteMySubmission>>(parsed.Value, null)
            : parsed.Error!.Value;
    }

    public async Task<VotingResult> DeleteMySubmissionAsync(long id)
    {
        if (await RequireSignIn() is { } error)
            return error;

        return ToResult(await transport.SendAsync(
            new VotingRequest(HttpVerb.Delete, $"/my/submissions/{id}") { Authed = true }));
    }

    /// <summary>
    /// Uploads art for a card. <paramref name="extension"/> is the file
    /// extension without the dot (png, jpg, jpeg, webp).
    /// </summary>
    public async Task<VotingResult> UploadAsync(string category, string entry, byte[] image, string extension)
    {
        var ext = extension.TrimStart('.').ToLowerInvariant();
        var mime = ext switch
        {
            "png" => "image/png",
            "jpg" or "jpeg" => "image/jpeg",
            "webp" => "image/webp",
            _ => null
        };

        if (mime == null)
            return new VotingError(VotingErrorCode.UnsupportedImageType);

        if (await RequireSignIn() is { } error)
            return error;

        var boundary = "----DownfallUpload" + Guid.NewGuid().ToString("N");
        var body = new List<byte>();

        void AddField(string name, string value) =>
            body.AddRange(Encoding.UTF8.GetBytes(
                $"--{boundary}\r\nContent-Disposition: form-data; name=\"{name}\"\r\n\r\n{value}\r\n"));

        AddField("category", category);
        AddField("entry", entry);

        body.AddRange(Encoding.UTF8.GetBytes(
            $"--{boundary}\r\nContent-Disposition: form-data; name=\"image\"; filename=\"upload.{ext}\"\r\n" +
            $"Content-Type: {mime}\r\n\r\n"));
        body.AddRange(image);
        body.AddRange(Encoding.UTF8.GetBytes($"\r\n--{boundary}--\r\n"));

        return ToResult(await transport.SendAsync(new VotingRequest(HttpVerb.Post, "/submissions")
        {
            Body = body.ToArray(),
            ContentType = $"multipart/form-data; boundary={boundary}",
            Authed = true,
        }));
    }

    // ---- Shared helpers ----

    private async Task<VotingError?> RequireSignIn() => await session.EnsureSignedInAsync() switch
    {
        LoginOutcome.Success => null,
        LoginOutcome.Unreachable => new VotingError(VotingErrorCode.LoginUnreachable),
        LoginOutcome.Banned => new VotingError(VotingErrorCode.LoginBanned),
        LoginOutcome.Expired => new VotingError(VotingErrorCode.LoginExpired),
        LoginOutcome.Timeout => new VotingError(VotingErrorCode.LoginTimeout),
        _ => new VotingError(VotingErrorCode.LoginFailed),
    };

    private static VotingError ErrorFor(VotingResponse r) => r.Status switch
    {
        0 => new VotingError(VotingErrorCode.Network),
        401 => new VotingError(VotingErrorCode.SessionExpired, 401),
        403 => new VotingError(VotingErrorCode.Banned, 403),
        413 => new VotingError(VotingErrorCode.TooLarge, 413),
        // 400s (wrong size, corrupt file, NSFW rejection...) carry a specific
        // server-authored { error } message.
        _ => new VotingError(VotingErrorCode.Server, r.Status, VotingParser.ParseServerError(r.Body)),
    };

    private static VotingResult ToResult(VotingResponse r) =>
        r.IsSuccess ? VotingResult.Ok : ErrorFor(r);

    private static VotingResult<T> Parse<T>(VotingResponse r, Func<string, T?> parse) where T : class
    {
        // Reads only count as success on exactly 200, as before.
        if (r.Status != 200)
            return ErrorFor(r);

        var value = parse(r.Body);
        return value != null ? value : new VotingError(VotingErrorCode.BadResponse, r.Status);
    }
}
