using Downfall.DownfallCode.Voting.Client;

namespace Downfall.DownfallCode.Voting;

/// <summary>
/// Identifies one feed: a server sort ("hot", "top", "new") plus the pool
/// filter it was requested with.
/// </summary>
public readonly record struct FeedKey(string Sort, string PoolKey)
{
    public static FeedKey For(string sort, IReadOnlySet<VotingPool> pools) =>
        new(sort, pools.Count == 0 ? "" : string.Join(",", pools.OrderBy(p => p)));
}

/// <summary>
/// One feed's loaded pages. <see cref="NextOffset"/> is the offset for the
/// following page, null once exhausted (or after a failed load).
/// </summary>
public sealed class ArtFeed(IReadOnlySet<VotingPool> pools)
{
    public IReadOnlySet<VotingPool> Pools { get; } = pools;

    public List<ArtEntry> Items { get; } = [];

    public int? NextOffset { get; set; } = 0;

    public bool Loading { get; set; }

    public bool CanLoadMore => !Loading && NextOffset != null;
}

/// <summary>
/// Per (sort, pool-set) page cache and pagination for the voting screen, kept
/// out of the screen node. Switching sort/pool back and forth re-renders from
/// the cache instead of re-fetching.
/// </summary>
public sealed class ArtFeedStore(VotingClient client, int pageSize)
{
    private readonly Dictionary<FeedKey, ArtFeed> _feeds = new();

    public ArtFeed GetOrCreate(FeedKey key, IReadOnlySet<VotingPool> pools)
    {
        if (_feeds.TryGetValue(key, out var feed))
            return feed;

        feed = new ArtFeed(pools);
        _feeds[key] = feed;
        return feed;
    }

    public ArtFeed? Get(FeedKey key) => _feeds.GetValueOrDefault(key);

    public void Clear() => _feeds.Clear();

    /// <summary>
    /// Fetches the feed's next page and appends it. Returns the newly loaded
    /// entries, or null if the request failed (the feed is then marked
    /// exhausted so it is not retried in a loop) or there was nothing to load.
    /// </summary>
    public async Task<IReadOnlyList<ArtEntry>?> LoadNextPageAsync(FeedKey key, ArtFeed feed)
    {
        if (!feed.CanLoadMore)
            return null;

        feed.Loading = true;
        var result = await client.GetFeedAsync(
            feed.Pools.Select(p => p.ToString()).ToList(), key.Sort, feed.NextOffset!.Value, pageSize);
        feed.Loading = false;

        if (!result.IsOk)
        {
            Godot.GD.PrintErr($"GetFeed failed: {result.Error}");
            feed.NextOffset = null;
            return null;
        }

        var items = result.Value!.Items.Select(VotingMapping.ToArtEntry).ToList();
        feed.NextOffset = result.Value.NextOffset;
        feed.Items.AddRange(items);
        return items;
    }

    /// <summary>
    /// A vote card's like/count state lives on its node; the cached
    /// <see cref="ArtEntry"/> is an immutable snapshot from fetch time. This
    /// keeps every cache in step (the same submission can sit in several
    /// feeds at once) so a like survives switching sort/pool and back.
    /// </summary>
    public void ApplyVote(long submissionId, bool liked, int upvotes)
    {
        foreach (var feed in _feeds.Values)
        {
            for (var i = 0; i < feed.Items.Count; i++)
            {
                if (feed.Items[i].Id != submissionId)
                    continue;

                feed.Items[i] = feed.Items[i] with { Liked = liked, Upvotes = upvotes };
                break;
            }
        }
    }
}
