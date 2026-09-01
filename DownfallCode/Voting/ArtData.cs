using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Voting;

public record ArtData
{
    public required ModelId ModelId { get; init; }
    public required string Id { get; init; }
    
    public CardModel? Card => ModelDb.GetByIdOrNull<CardModel>(ModelId);
}