using System.Text.Json;

namespace Downfall.DownfallCode.Voting.Client;

/// <summary>
/// The single place that knows the server's JSON field names. A schema change
/// on the server is an edit here. Missing optional fields get defaults;
/// missing required fields make the whole parse fail (null).
/// </summary>
public static class VotingParser
{
    public static FeedPage? ParseFeed(string json) => Guard(() =>
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var items = root.GetProperty("items").EnumerateArray().Select(ParseEntry).ToList();

        int? next = root.TryGetProperty("nextOffset", out var n) && n.ValueKind != JsonValueKind.Null
            ? (int)AsLong(n)
            : null;

        return new FeedPage(items, next);
    });

    public static List<RemoteCard>? ParseMissingCards(string json) => Guard(() =>
    {
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.ValueKind != JsonValueKind.Array)
            throw new JsonException("expected array");

        return doc.RootElement.EnumerateArray()
            .Select(d => new RemoteCard(Str(d, "category"), Str(d, "entry")))
            .ToList();
    });

    public static List<RemoteMySubmission>? ParseMySubmissions(string json) => Guard(() =>
    {
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.ValueKind != JsonValueKind.Array)
            throw new JsonException("expected array");

        return doc.RootElement.EnumerateArray()
            .Select(d => new RemoteMySubmission(
                Id: AsLong(d.GetProperty("id")),
                ImageUrl: OptStr(d, "image_url"),
                Status: Str(d, "status"),
                Category: Str(d, "category"),
                Entry: Str(d, "entry"),
                Upvotes: (int)AsLong(d.GetProperty("upvotes")),
                SubmittedAt: AsLong(d.GetProperty("created_at"))))
            .ToList();
    });

    public static MyProfile? ParseProfile(string json) => Guard(() =>
    {
        using var doc = JsonDocument.Parse(json);
        return new MyProfile(OptStr(doc.RootElement, "creditName"), OptStr(doc.RootElement, "steamId"));
    });

    /// <summary>The <c>{ "error": "..." }</c> message of a failed response, if it has one.</summary>
    public static string? ParseServerError(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var message = OptStr(doc.RootElement, "error");
            return string.IsNullOrEmpty(message) ? null : message;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static RemoteEntry ParseEntry(JsonElement d)
    {
        var flags = new HashSet<string>();
        if (d.TryGetProperty("my_flags", out var f) && f.ValueKind == JsonValueKind.Array)
        {
            foreach (var reason in f.EnumerateArray())
                flags.Add(reason.GetString() ?? "");
        }

        return new RemoteEntry(
            Id: AsLong(d.GetProperty("id")),
            Category: Str(d, "category"),
            Entry: Str(d, "entry"),
            ImageUrl: Str(d, "image_url"),
            Author: OptStr(d, "author") ?? "",
            Upvotes: (int)AsLong(d.GetProperty("upvotes")),
            Liked: d.TryGetProperty("my_vote", out var v) && AsBool(v),
            MyFlags: flags,
            SubmittedAt: d.TryGetProperty("created_at", out var c) ? AsLong(c) : 0);
    }

    // Numbers may arrive as 12 or 12.0 depending on the server's serializer.
    private static long AsLong(JsonElement e) => e.TryGetInt64(out var l) ? l : (long)e.GetDouble();

    private static bool AsBool(JsonElement e) => e.ValueKind switch
    {
        JsonValueKind.True => true,
        JsonValueKind.Number => e.GetDouble() != 0,
        _ => false
    };

    private static string Str(JsonElement e, string name) =>
        e.GetProperty(name).GetString() ?? throw new JsonException($"{name} is null");

    private static string? OptStr(JsonElement e, string name) =>
        e.ValueKind == JsonValueKind.Object && e.TryGetProperty(name, out var p) && p.ValueKind == JsonValueKind.String
            ? p.GetString()
            : null;

    private static T? Guard<T>(Func<T> parse) where T : class
    {
        try
        {
            return parse();
        }
        catch (Exception e) when (e is JsonException or KeyNotFoundException or InvalidOperationException
                                      or FormatException or OverflowException)
        {
            return null;
        }
    }
}
