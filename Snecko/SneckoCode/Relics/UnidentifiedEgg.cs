using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Relics;

[Pool(typeof(SneckoRelicPool))]
public class UnidentifiedEgg : SneckoRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    // TODO
}