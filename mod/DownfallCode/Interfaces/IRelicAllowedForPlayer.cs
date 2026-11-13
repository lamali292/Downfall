using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.DownfallCode.Interfaces;

/// <summary>
/// A relic that can only be offered to some players (e.g. it needs a suitable card in that player's deck).
/// The base game's <c>RelicModel.IsAllowed</c> only sees the run state and, worse, permanently removes a
/// disallowed relic from the grab bag, so this is checked per pull in <c>RelicPullPlayerFilterPatch</c>
/// instead: a relic that fails is just skipped for this pull and stays in the bag.
/// </summary>
public interface IRelicAllowedForPlayer
{
    bool IsAllowedForPlayer(Player player);
}
