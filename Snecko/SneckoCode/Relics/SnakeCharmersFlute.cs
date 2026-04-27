using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Relics;

[Pool(typeof(SneckoRelicPool))]
public class SnakeCharmersFlute : SneckoRelicModel
{
    // TODO - Boss relic
    public override RelicRarity Rarity => RelicRarity.Ancient;
}