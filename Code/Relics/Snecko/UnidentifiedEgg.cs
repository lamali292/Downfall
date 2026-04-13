using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Downfall.Code.Relics.Snecko;

[Pool(typeof(SneckoRelicPool))]
public class UnidentifiedEgg : SneckoRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    // TODO
}