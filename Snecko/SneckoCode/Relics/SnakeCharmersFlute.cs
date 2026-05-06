using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Events;

namespace Snecko.SneckoCode.Relics;

[Pool(typeof(SneckoRelicPool))]
public class SnakeCharmersFlute : SneckoRelicModel, IShouldAllowMuddleCost
{

    public SnakeCharmersFlute() : base(RelicRarity.Ancient)
    {
        WithTip(SneckoKeywords.Muddle);
    }

    public bool ShouldAllowMuddleCost(CardModel card, int cost) =>
        cost != 3 || card.Owner != Owner;
}