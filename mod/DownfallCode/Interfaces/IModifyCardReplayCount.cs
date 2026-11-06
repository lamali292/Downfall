using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Interfaces;

/// <summary>
///     Implemented by combat hook listeners (powers, relics, ...) that grant Replay to cards they don't own.
///     Applied inside <c>CardModel.GetEnchantedReplayCount</c>, so the card text/tooltip and the actual play count
///     both see it. For a card granting itself Replay, use <see cref="IModifyReplayCount" /> instead.
/// </summary>
public interface IModifyCardReplayCount
{
    int ModifyReplayCount(CardModel card, int current);
}
