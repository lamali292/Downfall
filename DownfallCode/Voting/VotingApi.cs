using System.Text;
using Godot;
using Godot.Collections;
using MegaCrit.Sts2.Core.Models;
using FileAccess = Godot.FileAccess;
using HttpClient = Godot.HttpClient;

namespace Downfall.DownfallCode.Voting;

public partial class VotingApi : Node
{
    private const string BaseUrl = "https://api.downfall-sts2.org/voting";

    // Shared secret Caddy expects on every request to this domain. Not a
    // real per-user credential - same threat model as the old Supabase
    // publishable key: it just keeps casual scraping/abuse off the API.
    private const string GateKey = "0asdj0asdj0a0j0q22pm";

    public static VotingApi Instance { get; private set; } = null!;

    private static string[] JsonHeaders =>
    [
        $"apikey: {GateKey}",
        "Content-Type: application/json"
    ];

    public override void _Ready()
    {
        Instance = this;
    }

    public async Task<List<ArtEntry>?> GetSubmissions(ArtData data)
    {
        var user = UserIdentity.Id;
        if (user == null)
        {
            GD.PrintErr("GetSubmissions skipped: no SteamID (Steam not running)");
            return null;
        }

        var (code, resp) = await Send(
            $"{BaseUrl}/submissions?category={Uri.EscapeDataString(data.ModelId.Category)}" +
            $"&entry={Uri.EscapeDataString(data.ModelId.Entry)}&user={Uri.EscapeDataString(user)}",
            HttpClient.Method.Get);

        if (code == 200)
            return Parse(resp, data);

        GD.PrintErr($"GetSubmissions {code}: {resp}");
        return null;
    }

    /// <summary>
    /// Every card currently open for art submission - admin-curated
    /// server-side (see voting_missing_art_cards), not inferred from the
    /// client's own asset state. Public/anonymous, same as browsing the
    /// feed. Returns an empty list on failure rather than null so callers
    /// (the card picker) degrade to "nothing to pick" instead of crashing.
    /// </summary>
    public async Task<List<ArtData>> GetMissingCards()
    {
        var (code, resp) = await Send($"{BaseUrl}/missing-cards", HttpClient.Method.Get);

        if (code != 200)
        {
            GD.PrintErr($"GetMissingCards {code}: {resp}");
            return [];
        }

        var parsed = Json.ParseString(resp);
        if (parsed.VariantType != Variant.Type.Array)
            return [];

        return parsed.AsGodotArray()
            .Select(item => item.AsGodotDictionary())
            .Select(d => new ArtData { ModelId = new ModelId(d["category"].AsString(), d["entry"].AsString()) })
            .ToList();
    }

    /// <summary>
    /// One page of a server-sorted feed across every card the voting screen
    /// wants to show at once (a single request instead of one per card).
    /// Missing-art status doesn't gate browsing/voting - only uploading (see
    /// <see cref="UploadSubmission"/>) - so this always returns approved
    /// submissions for every card, newest/hottest/top first, unless
    /// <paramref name="pools"/> narrows it to specific characters (empty =
    /// no filter). The server maps each pool name straight to that mod's
    /// known <c>ModelId.Entry</c> prefix - it doesn't need the client to
    /// enumerate individual cards. Returns null on failure, or (items, next
    /// offset to pass back in for the following page - null once exhausted).
    /// </summary>
    public async Task<(List<ArtEntry> items, int? nextOffset)?> GetSubmissionsFeed(
        IReadOnlySet<VotingPool> pools, string sort, int offset, int limit)
    {
        var user = UserIdentity.Id;
        if (user == null)
        {
            GD.PrintErr("GetSubmissionsFeed skipped: no SteamID (Steam not running)");
            return null;
        }

        var body = new Dictionary
        {
            { "user", user },
            { "sort", sort },
            { "offset", offset },
            { "limit", limit },
        };

        if (pools.Count > 0)
            body["pools"] = new Godot.Collections.Array(pools.Select(p => (Variant)p.ToString()).ToArray());

        var (code, resp) = await Send($"{BaseUrl}/submissions/feed", HttpClient.Method.Post, Json.Stringify(body));

        if (code != 200)
        {
            GD.PrintErr($"GetSubmissionsFeed {code}: {resp}");
            return null;
        }

        var parsed = Json.ParseString(resp).AsGodotDictionary();

        var items = new List<ArtEntry>();
        foreach (var item in parsed["items"].AsGodotArray())
        {
            var d = item.AsGodotDictionary();

            var flags = new HashSet<string>();
            if (d.ContainsKey("my_flags"))
            {
                foreach (var r in d["my_flags"].AsGodotArray())
                    flags.Add(r.AsString());
            }

            items.Add(new ArtEntry
            {
                Id = d["id"].AsInt64(),
                ImagePath = d["image_url"].AsString(),
                Author = d["author"].AsString(),
                Upvotes = d["upvotes"].AsInt32(),
                Liked = d["my_vote"].AsBool(),
                MyFlags = flags,
                SubmittedAt = d["created_at"].AsInt64(),
                ModelId = new ModelId(d["category"].AsString(), d["entry"].AsString()),
            });
        }

        var nextOffset = parsed["nextOffset"].VariantType == Variant.Type.Nil
            ? (int?)null
            : parsed["nextOffset"].AsInt32();

        return (items, nextOffset);
    }

    // Voting and flagging require a Steam-verified session (same one
    // uploading uses), not just the anonymous UserIdentity.Id hash - that
    // used to be all the server checked, which made votes/reports trivial
    // to sybil with made-up ids. EnsureSignedIn opens the Steam login page
    // the first time (session persists ~30 days after that), so this is a
    // one-off prompt in practice, not one per click.

    public async Task CastVote(long submissionId)
    {
        if (!await VotingAuth.EnsureSignedIn())
        {
            GD.PrintErr("CastVote skipped: not signed in with Steam");
            return;
        }

        var body = Json.Stringify(new Dictionary { { "submissionId", submissionId } });
        var (code, resp) = await SendAuthed($"{BaseUrl}/vote", HttpClient.Method.Post, VotingAuth.Token!, body);

        if (code is < 200 or > 299)
            GD.PrintErr($"CastVote {code}: {resp}");
    }

    public async Task ClearVote(long submissionId)
    {
        if (!await VotingAuth.EnsureSignedIn())
        {
            GD.PrintErr("ClearVote skipped: not signed in with Steam");
            return;
        }

        var body = Json.Stringify(new Dictionary { { "submissionId", submissionId } });
        var (code, resp) = await SendAuthed($"{BaseUrl}/unvote", HttpClient.Method.Post, VotingAuth.Token!, body);

        if (code is < 200 or > 299)
            GD.PrintErr($"ClearVote {code}: {resp}");
    }

    public async Task ToggleFlag(long submissionId, string reason, bool on)
    {
        if (!await VotingAuth.EnsureSignedIn())
        {
            GD.PrintErr("ToggleFlag skipped: not signed in with Steam");
            return;
        }

        var body = Json.Stringify(new Dictionary
        {
            { "submissionId", submissionId },
            { "reason", reason },
            { "on", on }
        });

        var (code, resp) = await SendAuthed($"{BaseUrl}/flag", HttpClient.Method.Post, VotingAuth.Token!, body);

        if (code is < 200 or > 299)
            GD.PrintErr($"ToggleFlag {code}: {resp}");
    }

    /// <summary>
    /// Applies every reason change from one "Report" submit in a single
    /// request - picking several reasons used to fire one <see cref="ToggleFlag"/>
    /// call per reason.
    /// </summary>
    public async Task ToggleFlags(long submissionId, IReadOnlyCollection<string> add, IReadOnlyCollection<string> remove)
    {
        if (add.Count == 0 && remove.Count == 0)
            return;

        if (!await VotingAuth.EnsureSignedIn())
        {
            GD.PrintErr("ToggleFlags skipped: not signed in with Steam");
            return;
        }

        var body = Json.Stringify(new Dictionary
        {
            { "submissionId", submissionId },
            { "add", new Godot.Collections.Array(add.Select(r => (Variant)r).ToArray()) },
            { "remove", new Godot.Collections.Array(remove.Select(r => (Variant)r).ToArray()) },
        });

        var (code, resp) = await SendAuthed($"{BaseUrl}/flag/batch", HttpClient.Method.Post, VotingAuth.Token!, body);

        if (code is < 200 or > 299)
            GD.PrintErr($"ToggleFlags {code}: {resp}");
    }

    // ---- Steam login (device-code-style: open browser, poll for completion) ----

    public async Task<(string? state, string? loginUrl)> StartSteamLogin()
    {
        var (code, resp) = await Send($"{BaseUrl}/auth/steam/start", HttpClient.Method.Post, "{}");

        if (code != 200)
        {
            GD.PrintErr($"StartSteamLogin {code}: {resp}");
            return (null, null);
        }

        var d = Json.ParseString(resp).AsGodotDictionary();
        return (d["state"].AsString(), d["loginUrl"].AsString());
    }

    public async Task<(string status, string? token)> PollSteamLogin(string state)
    {
        var (code, resp) = await Send(
            $"{BaseUrl}/auth/steam/poll?state={Uri.EscapeDataString(state)}",
            HttpClient.Method.Get);

        if (code == 403)
            return ("banned", null);

        if (code != 200)
        {
            GD.PrintErr($"PollSteamLogin {code}: {resp}");
            return ("error", null);
        }

        var d = Json.ParseString(resp).AsGodotDictionary();
        var status = d["status"].AsString();
        var token = status == "completed" ? d["token"].AsString() : null;
        return (status, token);
    }

    // ---- Artist credit name (one per Steam account, not per submission) ----

    /// <summary>
    /// The credit name previously saved for this account via
    /// <see cref="SetMyCreditName"/>, or null if none has been set yet.
    /// </summary>
    public async Task<string?> GetMyCreditName()
    {
        var token = VotingAuth.Token;
        if (token == null)
            return null;

        var (code, resp) = await SendAuthed($"{BaseUrl}/my/profile", HttpClient.Method.Get, token);

        if (code != 200)
        {
            GD.PrintErr($"GetMyCreditName {code}: {resp}");
            return null;
        }

        var d = Json.ParseString(resp).AsGodotDictionary();
        return d["creditName"].VariantType == Variant.Type.Nil ? null : d["creditName"].AsString();
    }

    /// <summary>
    /// Saves this account's art-credit name - it's looked up live wherever a
    /// submission is displayed (server-side, joined on steam_id), so this
    /// applies to every submission that account has ever made, not just
    /// future uploads. Renaming is server-rate-limited; a failed rename
    /// because of that comes back as <paramref name="error"/> rather than
    /// a generic failure.
    /// </summary>
    public async Task<(bool ok, string? error)> SetMyCreditName(string creditName)
    {
        var token = VotingAuth.Token;
        if (token == null)
            return (false, null);

        var body = Json.Stringify(new Dictionary { { "creditName", creditName } });
        var (code, resp) = await SendAuthed($"{BaseUrl}/my/profile", HttpClient.Method.Put, token, body);

        if (code is < 200 or > 299)
        {
            GD.PrintErr($"SetMyCreditName {code}: {resp}");
            return (false, TryGetServerErrorMessage(resp) ?? VotingUi.Loc("DOWNFALL-VOTING.error_credit_name_save_failed"));
        }

        return (true, null);
    }

    // ---- Upload (requires a Steam-verified session from VotingAuth) ----

    public async Task<(bool ok, string error)> UploadSubmission(
        ModelId modelId, string imagePath)
    {
        var token = VotingAuth.Token;
        if (token == null)
            return (false, VotingUi.Loc("DOWNFALL-VOTING.error_upload_not_signed_in"));

        using var file = FileAccess.Open(imagePath, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            return (false, VotingUi.Loc("DOWNFALL-VOTING.error_file_read",
                ("error", FileAccess.GetOpenError().ToString())));
        }

        var fileBytes = file.GetBuffer((long)file.GetLength());

        var ext = imagePath.GetExtension().ToLowerInvariant();
        var mime = ext switch
        {
            "png" => "image/png",
            "jpg" or "jpeg" => "image/jpeg",
            "webp" => "image/webp",
            _ => null
        };

        if (mime == null)
            return (false, VotingUi.Loc("DOWNFALL-VOTING.error_unsupported_type"));

        var boundary = "----DownfallUpload" + Guid.NewGuid().ToString("N");
        var body = new List<byte>();

        void AddField(string name, string value)
        {
            body.AddRange(Encoding.UTF8.GetBytes(
                $"--{boundary}\r\nContent-Disposition: form-data; name=\"{name}\"\r\n\r\n{value}\r\n"));
        }

        AddField("category", modelId.Category);
        AddField("entry", modelId.Entry);

        body.AddRange(Encoding.UTF8.GetBytes(
            $"--{boundary}\r\nContent-Disposition: form-data; name=\"image\"; filename=\"upload.{ext}\"\r\n" +
            $"Content-Type: {mime}\r\n\r\n"));
        body.AddRange(fileBytes);
        body.AddRange(Encoding.UTF8.GetBytes($"\r\n--{boundary}--\r\n"));

        string[] headers =
        [
            $"apikey: {GateKey}",
            $"Authorization: Bearer {token}",
            $"Content-Type: multipart/form-data; boundary={boundary}"
        ];

        var http = new HttpRequest();
        AddChild(http);

        var err = http.RequestRaw($"{BaseUrl}/submissions", headers, HttpClient.Method.Post, body.ToArray());

        if (err != Error.Ok)
        {
            http.QueueFree();
            return (false, $"Request failed to send: {err}");
        }

        var result = await ToSignal(http, HttpRequest.SignalName.RequestCompleted);
        http.QueueFree();

        var code = result[1].AsInt64();
        var text = Encoding.UTF8.GetString(result[3].AsByteArray());

        if (code is >= 200 and < 300)
            return (true, "");

        DownfallMainFile.Logger.Info($"[VotingApi] upload failed {code}: {text}");
        return (false, DescribeUploadError(code, text));
    }

    // ---- Self-service: viewing/withdrawing your own submissions ----

    public async Task<List<MySubmission>?> GetMySubmissions()
    {
        var token = VotingAuth.Token;
        if (token == null)
            return null;

        var (code, resp) = await SendAuthed($"{BaseUrl}/my/submissions", HttpClient.Method.Get, token);

        if (code != 200)
        {
            GD.PrintErr($"GetMySubmissions {code}: {resp}");
            return null;
        }

        var parsed = Json.ParseString(resp);
        if (parsed.VariantType != Variant.Type.Array)
            return null;

        return parsed.AsGodotArray()
            .Select(item => item.AsGodotDictionary())
            .Select(d => new MySubmission
            {
                Id = d["id"].AsInt64(),
                ImagePath = d["image_url"].VariantType == Variant.Type.Nil ? null : d["image_url"].AsString(),
                Status = d["status"].AsString(),
                ModelId = new ModelId(d["category"].AsString(), d["entry"].AsString()),
                Upvotes = d["upvotes"].AsInt32(),
                SubmittedAt = d["created_at"].AsInt64(),
            })
            .ToList();
    }

    public async Task<bool> DeleteMySubmission(long id)
    {
        var token = VotingAuth.Token;
        if (token == null)
            return false;

        var (code, resp) = await SendAuthed($"{BaseUrl}/my/submissions/{id}", HttpClient.Method.Delete, token);

        if (code is < 200 or > 299)
        {
            GD.PrintErr($"DeleteMySubmission {code}: {resp}");
            return false;
        }

        return true;
    }

    private static string DescribeUploadError(long code, string body)
    {
        // 401/403/413 are plain HTTP statuses with no useful body, so those
        // get a fixed localized message. Everything else (400s in
        // particular - wrong size, corrupt file, NSFW-filter rejection...)
        // carries a specific server-authored { error } message that's more
        // useful than a generic "upload failed" - prefer that when present.
        return code switch
        {
            401 => VotingUi.Loc("DOWNFALL-VOTING.error_upload_session_expired"),
            403 => VotingUi.Loc("DOWNFALL-VOTING.error_login_banned"),
            413 => VotingUi.Loc("DOWNFALL-VOTING.error_upload_too_large"),
            _ => TryGetServerErrorMessage(body) ?? VotingUi.Loc("DOWNFALL-VOTING.error_upload_generic", ("code", code.ToString())),
        };
    }

    private static string? TryGetServerErrorMessage(string body)
    {
        try
        {
            var parsed = Json.ParseString(body);
            var dict = parsed.AsGodotDictionary();
            return dict.ContainsKey("error") ? dict["error"].AsString() : null;
        }
        catch
        {
            return null;
        }
    }

    private static List<ArtEntry>? Parse(string json, ArtData data)
    {
        var parsed = Json.ParseString(json);

        if (parsed.VariantType != Variant.Type.Array)
            return null;

        var list = new List<ArtEntry>();

        foreach (var item in parsed.AsGodotArray())
        {
            var d = item.AsGodotDictionary();

            var flags = new HashSet<string>();

            if (d.ContainsKey("my_flags"))
            {
                foreach (var r in d["my_flags"].AsGodotArray())
                    flags.Add(r.AsString());
            }

            list.Add(new ArtEntry
            {
                Id = d["id"].AsInt64(),
                ImagePath = d["image_url"].AsString(),
                Author = d["author"].AsString(),
                Upvotes = d["upvotes"].AsInt32(),
                Liked = d["my_vote"].AsBool(),
                MyFlags = flags,
                SubmittedAt = d["created_at"].AsInt64(),
                ModelId = data.ModelId
            });
        }

        return list;
    }

    private Task<(long code, string body)> SendAuthed(
        string url,
        HttpClient.Method method,
        string token,
        string body = "")
    {
        string[] headers =
        [
            $"apikey: {GateKey}",
            $"Authorization: Bearer {token}",
            "Content-Type: application/json"
        ];

        return Send(url, method, body, headers);
    }

    // Requests/responses that list every card (submissions/feed) can run
    // into the tens of KB; logging them in full just to see "-> POST .../vote"
    // elsewhere buries the log in noise, so anything past this gets cut off.
    private const int LogBodyLimit = 300;

    private async Task<(long code, string body)> Send(
        string url,
        HttpClient.Method method,
        string body = "",
        string[]? headers = null)
    {
        DownfallMainFile.Logger.Info(
            $"[VotingApi] -> {method} {url} {Truncate(body)}");

        var http = new HttpRequest();
        AddChild(http);

        var err = http.Request(url, headers ?? JsonHeaders, method, body);

        if (err != Error.Ok)
        {
            http.QueueFree();

            DownfallMainFile.Logger.Info(
                $"[VotingApi] <- request failed to send: {err}");

            return (0, "request failed");
        }

        var result = await ToSignal(
            http,
            HttpRequest.SignalName.RequestCompleted);

        http.QueueFree();

        var code = result[1].AsInt64();
        var text = Encoding.UTF8.GetString(
            result[3].AsByteArray());

        DownfallMainFile.Logger.Info(
            $"[VotingApi] <- {code} {url} :: {Truncate(text)}");

        return (code, text);
    }

    private static string Truncate(string text) =>
        text.Length > LogBodyLimit ? $"{text[..LogBodyLimit]}... ({text.Length} chars)" : text;
}
