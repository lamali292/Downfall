using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Voting;

/// <summary>
/// A card that's open for art submissions. Unlike votes/flags, this isn't
/// server-curated - the client decides which cards qualify (see
/// <see cref="NArtVotingScreen"/>) by checking which cards still show their
/// "todo.tres" placeholder portrait, and identifies them by their real
/// <see cref="ModelId"/> rather than a server-assigned id.
/// </summary>
public record ArtData
{
    public required ModelId ModelId { get; init; }

    public CardModel? Card => ModelDb.GetByIdOrNull<CardModel>(ModelId);
}