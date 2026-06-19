namespace Downfall.DownfallCode.Voting;

public record ArtEntry
{
    public long Id { get; init; }
    public required string ImagePath { get; init; }
    public required string Author { get; init; }
    public required string Name { get; init; }
    public int Upvotes { get; init; }
    public int Downvotes { get; init; }
    public int MyVote { get; init; }                 // -1, 0, +1
    public HashSet<string> MyFlags { get; init; } = [];   // reasons this user flagged
}