using System.Text;
using Godot;
using Godot.Collections;
using MegaCrit.Sts2.Core.Models;
using HttpClient = Godot.HttpClient;

namespace Downfall.DownfallCode.Voting;

public partial class VotingApi : Node
{
    private const string BaseUrl = "https://njndpcayvomsutxgrezp.supabase.co/rest/v1";
    private const string Key = "sb_publishable_YkFWtYobqAQ9CZY7VzSHNg_dEXVlP1N";

    public static VotingApi Instance { get; private set; } = null!;

    private static string[] Headers =>
    [
        $"apikey: {Key}",
        $"Authorization: Bearer {Key}",
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

        var body = Json.Stringify(new Dictionary
        {
            { "p_category", data.Id },
            { "p_user", user }
        });

        var (code, resp) = await Send(
            $"{BaseUrl}/rpc/submissions_for_user",
            HttpClient.Method.Post,
            body);

        if (code == 200)
            return Parse(resp, data);

        GD.PrintErr($"GetSubmissions {code}: {resp}");
        return null;
    }

    public async Task CastVote(long submissionId)
    {
        var user = UserIdentity.Id;
        if (user == null)
        {
            GD.PrintErr("CastVote skipped: no SteamID (Steam not running)");
            return;
        }

        var body = Json.Stringify(new Dictionary
        {
            { "p_submission", submissionId },
            { "p_user", user }
        });

        var (code, resp) = await Send(
            $"{BaseUrl}/rpc/cast_vote",
            HttpClient.Method.Post,
            body);

        if (code is < 200 or > 299)
            GD.PrintErr($"CastVote {code}: {resp}");
    }

    public async Task ClearVote(long submissionId)
    {
        var user = UserIdentity.Id;
        if (user == null)
        {
            GD.PrintErr("ClearVote skipped: no SteamID");
            return;
        }

        var body = Json.Stringify(new Dictionary
        {
            { "p_submission", submissionId },
            { "p_user", user }
        });

        var (code, resp) = await Send(
            $"{BaseUrl}/rpc/clear_vote",
            HttpClient.Method.Post,
            body);

        if (code is < 200 or > 299)
            GD.PrintErr($"ClearVote {code}: {resp}");
    }

    public async Task ToggleFlag(long submissionId, string reason, bool on)
    {
        var user = UserIdentity.Id;
        if (user == null)
        {
            GD.PrintErr("ToggleFlag skipped: no SteamID (Steam not running)");
            return;
        }

        var body = Json.Stringify(new Dictionary
        {
            { "p_submission", submissionId },
            { "p_user", user },
            { "p_reason", reason },
            { "p_on", on }
        });

        var (code, resp) = await Send(
            $"{BaseUrl}/rpc/toggle_flag",
            HttpClient.Method.Post,
            body);

        if (code is < 200 or > 299)
            GD.PrintErr($"ToggleFlag {code}: {resp}");
    }

    public async Task<List<ArtData>?> GetCategories()
    {
        var (code, body) = await Send(
            $"{BaseUrl}/categories?order=id",
            HttpClient.Method.Get);

        if (code != 200)
        {
            GD.PrintErr($"GetCategories {code}: {body}");
            return null;
        }

        var parsed = Json.ParseString(body);

        if (parsed.VariantType != Variant.Type.Array)
            return null;

        return parsed.AsGodotArray()
            .Select(item => item.AsGodotDictionary())
            .Select(d => new ArtData
            {
                Id = d["id"].AsInt64().ToString(),
                ModelId = new ModelId(
                    d["category"].AsString(),
                    d["entry"].AsString())
            })
            .ToList();
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
                Name = d["name"].AsString(),
                Upvotes = d["upvotes"].AsInt32(),
                Liked = d["my_vote"].AsBool(),
                MyFlags = flags,
                SubmittedAt = d["created_at"].AsInt64(),
                ModelId = data.ModelId
            });
        }

        return list;
    }

    private async Task<(long code, string body)> Send(
        string url,
        HttpClient.Method method,
        string body = "")
    {
        DownfallMainFile.Logger.Info(
            $"[VotingApi] -> {method} {url} {body}");

        var http = new HttpRequest();
        AddChild(http);

        var err = http.Request(url, Headers, method, body);

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
            $"[VotingApi] <- {code} {url} :: {text}");

        return (code, text);
    }
}