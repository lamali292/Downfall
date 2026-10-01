using Downfall.DownfallCode.Voting.Client;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Voting;

/// <summary>Turns the client's plain DTOs into the game-facing records (which carry a <see cref="ModelId"/>).</summary>
public static class VotingMapping
{
    public static ArtEntry ToArtEntry(RemoteEntry e) => new()
    {
        Id = e.Id,
        ModelId = new ModelId(e.Category, e.Entry),
        ImagePath = e.ImageUrl,
        Author = e.Author,
        Upvotes = e.Upvotes,
        Liked = e.Liked,
        MyFlags = [..e.MyFlags],
        SubmittedAt = e.SubmittedAt,
    };

    public static ArtData ToArtData(RemoteCard c) => new() { ModelId = new ModelId(c.Category, c.Entry) };

    public static MySubmission ToMySubmission(RemoteMySubmission s) => new()
    {
        Id = s.Id,
        ImagePath = s.ImageUrl,
        Status = s.Status,
        ModelId = new ModelId(s.Category, s.Entry),
        Upvotes = s.Upvotes,
        SubmittedAt = s.SubmittedAt,
    };
}
