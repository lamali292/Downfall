using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Voting;

/// <summary>
/// One of the current player's own art submissions, for the self-service
/// "My Submissions" panel - lets them see review status and withdraw
/// something they uploaded, without needing the moderation admin panel.
/// </summary>
public record MySubmission
{
    public required long Id { get; init; }
    public required string? ImagePath { get; init; }
    public required string Name { get; init; }
    public required string Status { get; init; }
    public required ModelId ModelId { get; init; }
    public required int Upvotes { get; init; }
    public required long SubmittedAt { get; init; }

    public CardModel? Card => ModelDb.GetByIdOrNull<CardModel>(ModelId);
}
