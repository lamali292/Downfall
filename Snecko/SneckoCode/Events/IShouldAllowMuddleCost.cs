using MegaCrit.Sts2.Core.Models;

namespace Snecko.SneckoCode.Events;

public interface IShouldAllowMuddleCost
{
    bool ShouldAllowMuddleCost(CardModel card, int cost);
}