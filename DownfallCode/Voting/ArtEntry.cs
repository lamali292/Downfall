using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Voting;

public record ArtEntry
{
    public long Id { get; init; }
    public required ModelId ModelId { get; init; }
    public required string ImagePath { get; init; }
    public required string Author { get; init; }
    public required string Name { get; init; }
    public required long SubmittedAt { get; init; }
    public required int Upvotes { get; init; }
    public required bool Liked { get; init; } 
    public HashSet<string> MyFlags { get; init; } = [];
 
    public CardModel? Card =>
        ModelDb.GetByIdOrNull<CardModel>(ModelId);
}