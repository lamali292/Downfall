using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Voting;

/// <summary>
/// A card that's open for art submissions. Server-curated (see
/// <see cref="VotingApi.GetMissingCards"/>, backed by
/// <c>voting_missing_art_cards</c>) - not something the client infers from
/// its own asset state. Identified by its real <see cref="ModelId"/> rather
/// than a server-assigned id.
/// </summary>
public record ArtData
{
    public required ModelId ModelId { get; init; }

    public CardModel? Card => ModelDb.GetByIdOrNull<CardModel>(ModelId);
}