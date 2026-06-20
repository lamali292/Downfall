using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Voting;

public record ArtData
{
    public required ModelId ModelId { get; init; }
    public string? Id { get; init; }         
    public List<ArtEntry>? Entries;       
    
    
    public CardModel? Card => ModelDb.GetByIdOrNull<CardModel>(ModelId);
}